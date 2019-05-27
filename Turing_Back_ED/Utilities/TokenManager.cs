using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Turing_Back_ED.DAL;
using Turing_Back_ED.DomainModels;

namespace Turing_Back_ED.Utilities
{
    public class TokenManager
    {
        readonly LocalCache localCache;

        public TokenManager(LocalCache _localCache)
        {
            localCache = _localCache;
        }

        private string HttpToken(HttpRequest request)
        {
            request.Headers.TryGetValue("USER-KEY", out StringValues userKey);

            var tokenJWT = userKey.First().Split(" ")[1];

            return SecretHasher.GetHashWithSalt(tokenJWT);
        }

        /// <summary>
        /// Get the value of a cached record using its key
        /// which is an ecrypted token
        /// </summary>
        /// <param name="encryptedToken"></param>
        /// <returns></returns>
        public object GetValue(string encryptedToken)
        {
            var storedToken = localCache.Session[encryptedToken];
            if(storedToken.GetType() == typeof(int))
            {
                return (int)storedToken;
            }

            return null;
        }

        /// <summary>
        /// Get the value of a cached record by first querying
        /// the token from an HttpRequest, the use the recieved
        /// token to get the requested value
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object GetValueByRequest(HttpRequest request)
        {
            var encryptedToken = HttpToken(request);
            var storedToken = localCache.Session[encryptedToken];
            if (storedToken.GetType() == typeof(int))
            {
                return (int)storedToken;
            }

            return null;
        }

        /// <summary>
        /// Save a value using token from the request 
        /// object as key
        /// </summary>
        /// <param name="request"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool SaveValueByRequestToken(HttpRequest request, object value)
        {
            var encryptedToken = HttpToken(request);
            localCache.Session[encryptedToken] = value;
            return localCache.Session[encryptedToken] != null;
        }

        //private bool GetSaveCustomerIdWithNewToken(TokenSection tokenSection, RegisterModel model, object userId, object value)
        //{
        //    var encryptedToken = GetNewToken(tokenSection, model, userId);
        //    localCache.Session[encryptedToken] = value;
        //    return localCache.Session[encryptedToken] != null;
        //}

        public string GetNewToken(TokenSection tokenSection, string name, string email, object userId)
        {
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String((string)tokenSection.SignKey));
            var token = new JwtSecurityToken(
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
                issuer: tokenSection.Issuer,
                audience: tokenSection.Audience,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(Constants.DefaultExpiryInHours),
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.NameId, name),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString())
                }
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool SaveNewToken(object value)
        {
            var encryptedToken = string.Empty;//HttpToken(request);
            localCache.Session[encryptedToken] = value;
            return localCache.Session[encryptedToken] != null;
        }
    }
}
