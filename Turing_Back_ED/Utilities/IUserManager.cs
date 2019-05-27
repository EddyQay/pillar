using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;

namespace Turing_Back_ED.Utilities
{
    public interface IUserManager
    {
        UserModel AddUser();
        bool RemoveUser();
        UserModel UpdateUser();
        UserModel FindUser();
    }
}
