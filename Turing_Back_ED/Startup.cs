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
using Turing_Back_ED.Workers;
using System.IO;
using System.Net;
using Turing_Back_ED.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;

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
            //Add a context that will be the main gateway to the database.
            //Configure the context with a connection string from the appsettings.json file
            services.AddDbContext<DatabaseContext>(options =>
                options.UseMySql(Configuration.GetConnectionString(Constants.TuringDbConnectionName)));//use mysql database

            //Add MVC pattern to the application
            services.AddMvc().ConfigureApiBehaviorOptions(options => {
                //When this is set to false, .Net core will
                //automatically handle model validations.
                //True means that we'll manually handle model validation
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

            //We're setting up our user classes for dependency injection;
            //A pattern that allows instantion of all classes in one place
            //without having to use the 'new' keyword when creating objects of the class
            //AddTransient = Create an instance for every single use in a request
            //AddScoped = Create one instance for every single request
            //AddSingleton = Once instance for the whole application
            services.AddTransient<LocalCache, LocalCache>();
            services.AddTransient<TokenManager, TokenManager>();
            services.Configure<TokenSection>(Configuration.GetSection("TokenSection"));
            services.AddScoped<IAuthenticationManager, JwtAuthenticationManager>();
            services.AddScoped<IUserManager, LeanUserManager>();
            services.AddScoped<DatabaseContext, DatabaseContext>();
            services.AddTransient<IStore<Product>, ProductsWorker>();
            services.AddTransient<ProductsWorker, ProductsWorker>();
            services.AddScoped<CustomerWorker, CustomerWorker>();
            services.AddScoped<DepartmentsWorker, DepartmentsWorker>();
            services.AddScoped<TaxWorker, TaxWorker>();
            services.AddScoped<AttributesWorker, AttributesWorker>();
            services.AddScoped<ShippingsWorker, ShippingsWorker>();
            services.AddScoped<CategoriesWorker, CategoriesWorker>();
            services.AddScoped<ShoppingCartsWorker, ShoppingCartsWorker>();
            services.AddScoped<OrdersWorker, OrdersWorker>();
            services.AddScoped<PaymentsWorker, PaymentsWorker>();
            services.AddScoped<SecretHasher, SecretHasher>();
            services.AddScoped<ModelValidateAttribute>();
            services.AddTransient<SecretHasher, SecretHasher>();

            //get the token section from the configuration settings(appsettings.json)
            var tokenSection = Configuration.GetSection("TokenSection").Get<TokenSection>();

            //deconstruct the signing key from base64string to byte array
            var secret = Base64UrlEncoder.DecodeBytes((string)tokenSection.SignKey);

            services
                //Add authentication with options for the entire application
                .AddAuthentication(o => {
                    //set the default authentication scheme to Json Web Tokens(JWT),
                    //so we'll use by jwt by default for authentication 
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    
                })

                //Add JWT support for authentication
                .AddJwtBearer(o => {
                    o.SaveToken = true; //all tokens will be saved/ persisted
                    o.RequireHttpsMetadata = true; //okens will only be generated over secure (https) connections
                   
                    //Since we're validating tokens manually,
                    //we'll need to cinfgure how the tokens will be validated/
                    //when a user sends a token in a request to the api
                    o.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateTokenReplay = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,

                        ValidIssuer = tokenSection.Issuer,
                        
                        //Create a symmetric key from the bytes of our SigningKey in the settings file 
                        //Our tokens will be signed with symmetric cryptographic keys (public and private keys)
                        //The public key will be added to the token, and validated in here with the private key,
                        //which is known to only this app
                        IssuerSigningKey = new SymmetricSecurityKey(secret),

                        ValidAudience = tokenSection.Audience,
                    };

                    //We need to handle what happens when a user presents a token for authentication
                    o.Events = new JwtBearerEvents()
                    {
                        //when request is received
                        OnMessageReceived = (context) =>
                        {
                            //try to get the token from the request' 'USER-KEY' header
                            if (!context.Request.Headers.TryGetValue("USER-KEY", out StringValues userKey))
                            {
                                //if we didn't get anything from the 'USER-KEY' header, then it's likely
                                //he placed it in the Authorization header. But we don't the app to accept that,
                                //so
                                if (context.Request.Headers.TryGetValue("Authorization", out userKey))
                                {
                                    //if the toekn was found in the authorization header
                                    //explicitly reject it. Else, the app will accept it
                                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                    context.Fail(Constants.AuthCodeEmptyErrorMessage);
                                }

                                //If we got this far, then we didn't find any token at all.
                                //We'll leave the authentication module to handle this
                                return Task.CompletedTask;
                            }

                            //We got here, meaning, we found a token in the right place.
                            //Now let's see if it's a valid one
                            var requestToken = userKey.First();

                            //Now let's split the string we got from
                            //the USER-KEY header into 'Bearer' and '<the-real-token>'
                            var tokenizedToken = requestToken?.Split(" ");

                            //We're expecting the fist value in the array to be = "Bearer"
                            var bearer = tokenizedToken?[0];

                            //And the second value should be the real token
                            var tokenJWT = tokenizedToken?[1];

                            //Did the values match our expectation?
                            if (bearer == "Bearer" && !string.IsNullOrWhiteSpace(tokenJWT))
                            {
                                //Yes, it did. 
                                //Now because we told the handler we'll get him the token
                                //ourselves, we'll give him the token now,
                                //so he can take it up from there.
                                context.Token = tokenJWT;
                                return Task.CompletedTask;
                            }

                            //If we got here, our user's token didn't match our expectation,
                            //so we won't allow access
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Fail(Constants.AuthCodeEmptyErrorMessage);
                            return Task.CompletedTask;


                        },

                        //When a token has been sucessfully validated
                        OnTokenValidated = (context) =>
                        {
                            //claims found in the token are stored in the context object
                            //Now we'll get the 'UniqueName' claim, cos that's where
                            //we placed the user's/ customer's id while creating the token
                            if (context.Principal.FindFirst("UniqueName") is Claim uniqueNameClaim)
                            {
                                ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;

                                //Now we want to place the id in a claim with a more friendly name 'UserId'
                                //which corresponds to the first class property 'Name' of the Idenity object in 
                                //every .net context request object. The we can access directly
                                //when we need it
                                identity.AddClaim(new Claim("UserId", uniqueNameClaim.Value,ClaimValueTypes.Integer32));
                            }

                            return Task.CompletedTask;
                        },

                        //When token validation failed,
                        //either beacause the token was invalid,
                        //or because a token was not found
                        OnChallenge = (context) =>
                        {
                            //Usuallly our authentication module would like to handle this,
                            //but we need to send a custom error response,
                            //we tell him we'll handle it by ourselves
                            context.HandleResponse();

                            //We build a custom response object for our customer
                            var response = JToken.FromObject(new ErrorRequestModel()
                            {
                                Code = context.AuthenticateFailure != null
                                    ? Constants.ErrorCodes.AUT_02.ToString("g") //if the token is invalid or expired
                                    : Constants.ErrorCodes.AUT_01.ToString("g"),//if a token was not found
                                Message = context.AuthenticateFailure != null 
                                    ? context.AuthenticateFailure.Message
                                    : Constants.AuthCodeEmptyErrorMessage,
                                Status = (int)HttpStatusCode.Unauthorized
                            });

                            //Write the response to the HttpResponse object
                            return context.Response.WriteAsync(response.ToString());
                        }
                    };
                });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Pillar Shopping API",
                    Description = "A concise documentation on Pillar, an E-Commerce API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Edwin Ayi",
                        Email = "eddyka90@gmail.com",
                        Url = "https://twitter.com/edwin.ayi",
                    }
                    //License = new License
                    //{
                    //    Name = "Use under LICX",
                    //    Url = "https://example.com/license"
                    //}


                });

                //c.AddSecurityRequirement("HTTPS", "")

                c.AddSecurityDefinition("HTTPS", new ApiKeyScheme()
                {
                    Description = "User Security",
                    Name = "USER-KEY",
                    In = "Header",
                });

                
                //Let swagger generate documentation comments from xml documentation comments 
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                //c.RouteTemplate = "docs/{docummentName}/docs.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pillar API V1");
                c.RoutePrefix = string.Empty;
                c.EnableDeepLinking();
                c.EnableValidator();
               
            });

            
            app.UseMvc();
        }
    }
}
