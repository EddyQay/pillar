using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;

namespace Turing_Back_ED.Utilities
{
    public class JwtAuthenticationManager : IAuthenticationManager
    {
        private readonly TokenSection JwtTokenSection;

        public JwtAuthenticationManager(IOptions<TokenSection> _tokenSection)
        {
            JwtTokenSection = _tokenSection.Value;
        }

        public bool IsValidUser(LoginModel model)
        {
            return (model.Email == "myname" && model.Password == "abbacd")
                ? true
                : false;
        }

        public string SignIn(LoginModel model)
        {
            return Generators.GetJWTToken(JwtTokenSection, model);
        }

        public bool SignOut(LoginModel model)
        {
            throw new NotImplementedException();
        }
    }
}
