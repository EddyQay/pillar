using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Turing_Back_ED.Models;

namespace Turing_Back_ED.DomainModels
{
    public partial class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public partial class RegisterModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { set; get; }
    }

    public partial class RegisterResponseModel
    {
        [Required]
        public CustomerNoPass Customer { get; set; }

        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string Expires_In { get; set; }
    }
}
