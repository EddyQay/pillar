using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Turing_Back_ED.Models;

namespace Turing_Back_ED.DomainModels
{
    public partial class UserModel: IdentityUser<int>
    {
        public int Customer_id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        [Column("email")]
        override public string Email { set; get; }
    }

    public partial class AuthResponseModel
    {
        public Customer Customer { get; set; }
        public string Token { get; set; }
        public int Expiry { get; set; } = Utilities.Constants.DefaultExpiryInHours;
        public string Message { get; set; }
        public string Code { get; set; }
    }
}
