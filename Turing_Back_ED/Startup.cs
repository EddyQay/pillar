using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Turing_Back_ED.Utilities;
using Turing_Back_ED.Models;
using Microsoft.EntityFrameworkCore;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.DAL;
using System.IO;
using System.Net;
using Turing_Back_ED.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Turing_Back_ED
{
    public class Startup
    {
        public Startup(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. W'll use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TuringshopContext>(options =>
                options.UseMySql(Configuration.GetConnectionString(Constants.TuringDbConnectionName)));

            services.AddMvc().ConfigureApiBehaviorOptions(options => {
                options.SuppressModelStateInvalidFilter = true;
                #region OLD ALTERNATIVE
                //options.InvalidModelStateResponseFactory = context =>
                //{
                //    var response = new ErrorRequestModel
                //    {
                //        Code = $"PRM_01",
                //        Status = StatusCodes.Status400BadRequest,
                //        Message = Constants.BadRequestMessage,
                //        Errors = context.ModelState.Values.SelectMany(a => a.Errors)
                //    };

                //    return new BadRequestObjectResult(response);
                //};
                #endregion
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddTransient<LocalCache, LocalCache>();
            services.AddTransient<TokenManager, TokenManager>();
            services.Configure<TokenSection>(Configuration.GetSection("TokenSection"));
            services.AddScoped<IAuthenticationManager, JwtAuthenticationManager>();
            services.AddScoped<IUserManager, LeanUserManager>();
            services.AddScoped<TuringshopContext, TuringshopContext>();
            services.AddTransient<IStore<Product>, ProductStore>();
            services.AddTransient<ProductStore, ProductStore>();
            services.AddScoped<CustomerStore, CustomerStore>();
            services.AddScoped<DepartmentStore, DepartmentStore>();
            services.AddScoped<TaxStore, TaxStore>();
            services.AddScoped<AttributeStore, AttributeStore>();
            services.AddScoped<ShippingStore, ShippingStore>();
            services.AddScoped<CategoryStore, CategoryStore>();
            services.AddScoped<SecretHasher, SecretHasher>();
            services.AddScoped<ModelValidateAttribute>();

            services.AddTransient<SecretHasher, SecretHasher>();

            var token = Configuration.GetSection("TokenSection").Get<TokenSection>();
            var secret = Base64UrlEncoder.DecodeBytes((string)token.SignKey);

            services
                .AddAuthentication(o => {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o => {
                    o.SaveToken = true;
                    o.RequireHttpsMetadata = true;
                    o.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateTokenReplay = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,

                        ValidIssuer = token.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(secret),
                        ValidAudience = token.Audience,
                    };

                    o.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = (context) =>
                        {
                            if (!context.Request.Headers.TryGetValue("USER-KEY", out StringValues userKey))
                            {
                                if(context.Request.Headers.TryGetValue("Authorization", out userKey))
                                {
                                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                    context.Fail(Constants.AuthCodeEmptyErrorMessage);
                                }

                                return Task.CompletedTask;
                            }

                            var requestToken = userKey.First();
                            var tokenizedToken = requestToken?.Split(" ");
                            var bearer = tokenizedToken?[0];
                            var tokenJWT = tokenizedToken?[1];

                            if (bearer != "Bearer" || string.IsNullOrWhiteSpace(tokenJWT))
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Fail(Constants.AuthCodeEmptyErrorMessage);
                                return Task.CompletedTask;
                            }

                            context.Token = tokenJWT;
                            return Task.CompletedTask;
                            //tokenJWT = tokenJWT.Substring(tokenJWT.IndexOf("Bearer ") + 1, tokenJWT.Length);
                        },

                        OnTokenValidated = (context) =>
                        {
                            if (context.Principal.FindFirst("UniqueName") is Claim uniqueNameClaim)
                            {
                                ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                                identity.AddClaim(new Claim("UserId", uniqueNameClaim.Value,ClaimValueTypes.Integer32));
                            }

                            return Task.CompletedTask;
                        },

                        OnChallenge = (context) =>
                        {
                            context.HandleResponse();
                            var response = JToken.FromObject(new ErrorRequestModel()
                            {
                                Code = context.AuthenticateFailure != null
                                    ? Constants.ErrorCodes.AUT_02.ToString("g")
                                    : Constants.ErrorCodes.AUT_01.ToString("g"),
                                Message = context.AuthenticateFailure != null 
                                    ? context.AuthenticateFailure.Message
                                    : Constants.AuthCodeEmptyErrorMessage,
                                Status = (int)HttpStatusCode.Unauthorized
                            });

                            return context.Response.WriteAsync(response.ToString());
                        }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            loggerFactory.AddFile($"Logs/Log.{DateTime.Today.ToShortDateString()}.txt");

            app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()).Build();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMiddleware<ErrorMiddleware>();
            app.UseMvc();
        }
    }
}
