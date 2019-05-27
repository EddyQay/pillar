using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Models;

namespace Turing_Back_ED.Utilities
{
    public class LeanUserManager : IUserManager
    {
        public readonly TuringshopContext _context;

        public LeanUserManager(TuringshopContext context)
        {
            _context = context;
        }

        public UserModel AddUser()
        {
            throw new NotImplementedException();
        }

        public UserModel FindUser()
        {
            throw new NotImplementedException();
        }

        public bool RemoveUser()
        {
            throw new NotImplementedException();
        }

        public UserModel UpdateUser()
        {
            throw new NotImplementedException();
        }
    }
}
