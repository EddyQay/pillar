using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;

namespace Turing_Back_ED.Utilities
{
    public class Generators
    {
        public static string GetJWTToken(TokenSection tokenSection, LoginModel model)
        {
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String((string)tokenSection.SignKey));
            var token = new JwtSecurityToken(
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
                issuer: tokenSection.Issuer,
                audience: tokenSection.Audience,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(24),
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                    new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString())
                }
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GetJWTToken(TokenSection tokenSection, RegisterModel model)
        {
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String((string)tokenSection.SignKey));
            var token = new JwtSecurityToken(
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
                issuer: tokenSection.Issuer,
                audience: tokenSection.Audience,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(24),
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Name),
                    new Claim(JwtRegisteredClaimNames.Email, model.Email),
                    new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString())
                }
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
