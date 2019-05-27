using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turing_Back_ED.DAL;

namespace Turing_Back_ED.Models
{
    public partial class CustomerForUpdate : IEntity
    {
        public int CustomerId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [MinLength(12)]
        public string Password { get; set; } = string.Empty;
        public string CreditCard { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int ShippingRegionId { get; set; }
        public string DayPhone { get; set; } = string.Empty;
        public string EvePhone { get; set; } = string.Empty;
        public string MobPhone { get; set; } = string.Empty;
        public DateTimeOffset? Added { get; set; }
        public DateTimeOffset? Modified { get; set; }

        public static explicit operator CustomerForUpdate(Customer v)
        {
            CustomerForUpdate cust = new CustomerForUpdate
            {
                CustomerId = v.CustomerId,
                Name = v.Name ?? string.Empty,
                Email = v.Email ?? string.Empty,
                MobPhone = v.MobPhone ?? string.Empty,
                DayPhone = v.DayPhone ?? string.Empty,
                EvePhone = v.EvePhone ?? string.Empty,
                Address1 = v.Address1 ?? string.Empty,
                Address2 = v.Address2 ?? string.Empty,
                City = v.City ?? string.Empty,
                Country = v.Country ?? string.Empty,
                CreditCard = v.CreditCard ?? string.Empty,
                Region = v.Region ?? string.Empty,
                ShippingRegionId = v.ShippingRegionId,
                PostalCode = v.PostalCode ?? string.Empty,
                Added = v.Added,
                Modified = v.Modified,
            };

            return cust;
        }
    }
}
