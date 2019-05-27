using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;

namespace Turing_Back_ED.Utilities
{
    public interface IAuthenticationManager
    {
        bool IsValidUser(LoginModel model);
        string SignIn(LoginModel model);
        bool SignOut(LoginModel model);
    }
}
