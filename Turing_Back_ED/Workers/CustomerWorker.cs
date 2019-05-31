using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Turing_Back_ED.Workers;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.DomainModels
{
    public class CustomerWorker : IStore<Customer>
    {
        private readonly DatabaseContext _context;
        private readonly TokenSection tokenSection;
        private readonly TokenManager tokenManager;

        public CustomerWorker(DatabaseContext context, TokenManager _tokenManager, 
            IOptions<TokenSection> _tokenSection) //from Startup's dependency injection
        {
            _context = context;
            tokenManager = _tokenManager;
            tokenSection = _tokenSection.Value;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync(GeneralQueryModel criteria)
        {
            IQueryable<Customer> searchResult = null;
            criteria = criteria ?? new GeneralQueryModel();

            searchResult = _context.Customers
                //for pagination eg. if page is given as 3, and
                //limit = 10, then pages to skip are 1 and 2
                //so skip = page -1 (that's equal to 2 pages)
                //then, the skip -> 2 x number of items per page
                //gives us number of items to skip, taking us
                //to where to start querying from.
                .Skip((int)((criteria.Page - 1) * criteria.Limit))
                //once we know where to start, we query
                //the item count specified in the 'Limit' param
                .Take((int)criteria.Limit);
            return await searchResult.ToListAsync();
        }

        public async Task<AuthResponseModel> AddAsync(Customer customer)
        {
            //Remember to validate email formats, credit cards, and the rest...
            Regex regex = EmailRegEx.EmailValidation();

            if(!regex.IsMatch(customer.Email))
            {
                return new AuthResponseModel()
                {
                    Customer = null,
                    Token = string.Empty,
                    Message = Constants.ErrorMessages.USR_03,
                    Code = Constants.ErrorCodes.USR_03.ToString("g")
                };
            }

            if(await _context.Customers
                .AnyAsync(a => a.Email == customer.Email))
            {
                return new AuthResponseModel() {
                    Customer = null,
                    Token = string.Empty,
                    Message = Constants.ErrorMessages.USR_04,
                    Code =  Constants.ErrorCodes.USR_04.ToString("g") };
            }

            customer.Password = SecretHasher.GetHashWithSalt(customer.Password);
            customer.Added = DateTime.UtcNow;
            _context.Customers.Add(customer).State = EntityState.Added;
            int result = await SaveChangesAsync();

            if (result < 1)
            {
                return new AuthResponseModel()
                {
                    Customer = null,
                    Token = string.Empty,
                    Message = Constants.ErrorMessages.SVR_00,
                    Code = Constants.ErrorCodes.SVR_00.ToString("g")
                };
            }

            var token = tokenManager.GetNewToken(tokenSection, customer.Name,
                    customer.Email, customer.CustomerId);

            return new AuthResponseModel()
            {
                Customer = customer,
                Token = $"Bearer {token}",
                Message = string.Empty,
                Code = string.Empty
            };
        }

        public async Task<AuthResponseModel> SignIn(LoginModel credentials)
        {
            //First look for a customer with the email provided
            var customerInStore = await _context.Customers.Where(c => c.Email.Equals(credentials.Email)).FirstOrDefaultAsync();

            //if no customer bears that name, return an error
            if (customerInStore == null)
            {
                return new AuthResponseModel()
                {
                    Customer = null,
                    Token = string.Empty,
                    Message = Constants.ErrorMessages.USR_01,
                    Code = Constants.ErrorCodes.USR_01.ToString("g")
                };
            }

            //If we got here, it only means a customer with that name was found.
            //Then, get that customer's password (already hashed)
            var userHashedPassword = customerInStore.Password;

            //compare this password to that of the user signing in.
            //During the comparison, the 'signing-in' user's password is hashed with same salt as
            //the customer-in-store's password
            bool passwordsMatched = SecretHasher.CompareHashes(userHashedPassword, credentials.Password);

            //if passwords did not match, return an error
            //Meaning, someone's lying, or made a mistake with credentials
            if(!passwordsMatched)
            {
                return new AuthResponseModel()
                {
                    Customer = null,
                    Token = string.Empty,
                    Message = Constants.ErrorMessages.USR_01,
                    Code = Constants.ErrorCodes.USR_01.ToString("g")
                };
            }

            //If we got here, then we got a match. Now generate a valid token
            //for our signned in customer
            var token = tokenManager.GetNewToken(tokenSection, customerInStore.Name, 
                customerInStore.Email, customerInStore.CustomerId);

            return new AuthResponseModel()
            {
                Customer = customerInStore,
                Token = $"Bearer {token}",
                Message = string.Empty,
                Code = string.Empty
            };
            
        }
        
        public async Task<Customer> FindByIdAsync(int Id)
        {
            return await _context.Customers.FindAsync(Id);
        }

        public async Task<Customer> UpdateAsync(Customer entity)
        {
            var customerInStore = await FindByIdAsync(entity.CustomerId);

            if(customerInStore != null)
            {
                var userHashedPassword = customerInStore.Password;

                //compare this password to that of the user signing in.
                //During the comparison, the 'signing-in' user's password is hashed with same salt as
                //the customer-in-store's password
                bool passwordsMatched = !string.IsNullOrWhiteSpace(entity.Password) 
                    ? SecretHasher.CompareHashes(userHashedPassword, entity.Password)
                    : true;

                //if passwords did not match, the customer has
                //changed their password. Hash the new password
                //with a different salt and save
                if (!passwordsMatched)
                {
                    entity.Password = SecretHasher.GetHashWithSalt(entity.Password);
                }

                customerInStore = customerInStore.UpdateWith(entity);
                _context.Entry(customerInStore).State = EntityState.Modified;
                await SaveChangesAsync();

                return customerInStore;
            }
            
            return null;
        }

        public async Task<Customer> UpdateAsync(string creditCard, int custId)
        {
            var customerInStore = await FindByIdAsync(custId);

            if (customerInStore != null)
            {
                if (!string.IsNullOrWhiteSpace(creditCard))
                {
                    StringBuilder builder = new StringBuilder();
                    var firstPortion = creditCard.Substring(0, creditCard.Length - 5);
                    var lastPortion = creditCard.Substring(creditCard.Length - 4, 4);
                    foreach (char item in firstPortion)
                    {
                        builder.Append('X');
                    }

                    builder.Append(lastPortion);
                    creditCard = builder.ToString();

                    customerInStore.CreditCard = creditCard;

                    _context.Entry(customerInStore).State = EntityState.Modified;
                    await SaveChangesAsync();

                }

                return customerInStore;
            }

            return null;
        }

        public Task<bool> IsValidCustomer(RegisterModel customer)
        {
            //Find the customer whose name matches the name and email provided.
            //Run this asynchronously
            return Task.Run(() =>
            {
                var thisCustomer = _context.Customers
                    .Where(c => c.Name.Equals(UrlEncoder.Default.Encode(customer.Name))
                    && c.Email.Equals(customer.Email)).Select(a => a)
                    .FirstOrDefault();

                //return false if no match was found
                if (thisCustomer == null)
                {
                    return false;
                }

                //return a comparison of the hash of the new password 
                //and the already existing hash
                return SecretHasher.CompareHashes(thisCustomer.Password, customer.Password);
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #region NOT IMPLEMENTED
        Task<Customer> IStore<Customer>.AddAsync(Customer entity)
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public async Task<IEnumerable<Customer>> FindAllAsync(SearchModel criteria)
        {
            IEnumerable<Customer> searchResult = null;
            switch (criteria.All_Words)
            {
                case "on":
                    searchResult = await _context.Customers
                        //find all products names that contain 'Query_String' value
                        .Where(p => p.Name.Contains(criteria.Query_String) ||
                        //or product description contains 'Query_String' value
                        p.Address1.Contains(criteria.Query_String))
                        //for pagination eg. if page is given as 3, and
                        //limit = 10, then pages to skip are 1 and 2
                        //so skip = page -1 (that's equal to 2)
                        //then, the skip (2) x number of items per page
                        //gives us number of items to skip, taking us
                        //to where to start querying from.
                        .Skip(criteria.Limit * (criteria.Page - 1))
                        //once we know where to start, we query
                        //the item count specified in the 'Limit' param 
                        .Take(criteria.Limit).ToListAsync();
                    break;
                case "off":
                    searchResult = await _context.Customers
                        //find all products whose names that DO NOT 
                        //contain 'Query_String' value
                        .Where(p => !p.Name.Contains(criteria.Query_String) &&
                        //AND descriptions DO NOT contain 'Query_String' value
                        !p.Address1.Contains(criteria.Query_String))
                        .Skip(criteria.Limit * (criteria.Page - 1))
                        .Take(criteria.Limit).ToListAsync();
                    break;
                default:
                    searchResult = await _context.Customers
                        .Where(p => p.Name.Contains(criteria.Query_String) ||
                        p.Address1.Contains(criteria.Query_String))
                        .Skip(criteria.Limit * (criteria.Page - 1))
                        .Take(criteria.Limit).ToListAsync();
                    break;
            }

            return searchResult;
        }

        public void Remove(Customer entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Customer>> FindByConditionAsync(Expression<Func<Customer, bool>> expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
