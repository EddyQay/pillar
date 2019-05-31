using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turing_Back_ED.Workers;

namespace Turing_Back_ED.Models
{
    public partial class Customer : IEntity
    {
        public int CustomerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(12)]
        [Required]
        public string Password { get; set; }
        public string CreditCard { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public int ShippingRegionId { get; set; }
        public string DayPhone { get; set; }
        public string EvePhone { get; set; }
        public string MobPhone { get; set; }
        public DateTimeOffset? Added { get; set; }
        public DateTimeOffset? Modified { get; set; }

        public static explicit operator Customer(CustomerForUpdate v)
        {
            Customer customer = new Customer
            {
                CustomerId = v.CustomerId,
                Name = v.Name ?? string.Empty,
                Email = v.Email ?? string.Empty,
                Password  = v.Password ?? string.Empty,
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

            return customer;
        }

        public Customer UpdateWith(Customer v)
        {
            Name = !string.IsNullOrWhiteSpace(v.Name) ? v.Name : Name;
            Email = !string.IsNullOrWhiteSpace(v.Email) ? v.Email : Email;
            Password = !string.IsNullOrWhiteSpace(v.Password) ? v.Password : Password;
            MobPhone = !string.IsNullOrWhiteSpace(v.MobPhone) ? v.MobPhone : MobPhone;
            DayPhone = !string.IsNullOrWhiteSpace(v.DayPhone) ? v.DayPhone : DayPhone;
            EvePhone = !string.IsNullOrWhiteSpace(v.EvePhone) ? v.EvePhone : EvePhone;
            Address1 = !string.IsNullOrWhiteSpace(v.Address1) ? v.Address1 : Address1;
            Address2 = !string.IsNullOrWhiteSpace(v.Address2) ? v.Address2 : Address2;
            City = !string.IsNullOrWhiteSpace(v.City) ? v.City : City;
            Country = !string.IsNullOrWhiteSpace(v.Country) ? v.Country : Country;
            CreditCard = !string.IsNullOrWhiteSpace(v.CreditCard) ? v.CreditCard : CreditCard;
            Region = !string.IsNullOrWhiteSpace(v.Region) ? v.Region : Region;
            ShippingRegionId = v.ShippingRegionId > 0 ? v.ShippingRegionId : ShippingRegionId;
            PostalCode = !string.IsNullOrWhiteSpace(v.PostalCode) ? v.PostalCode : PostalCode;
            Added = Added;
            Modified = DateTime.UtcNow;

            return this;
        }
    }
}
