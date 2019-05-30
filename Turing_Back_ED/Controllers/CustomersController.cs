using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Middlewares;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.Controllers
{
    /// <summary>
    /// Facilitates the creation an manipulation of customers
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerStore customers;

        public CustomersController(CustomerStore _customers)
        {
            customers = _customers;
        }

        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <param name="customer">A customer object to add</param>
        /// <returns>A Customer object</returns>
        [HttpPost]
        [ModelValidate]
        public async Task<ActionResult> AddCustomer([FromBody] Customer customer)
        {

            var authResponse = await customers.AddAsync(customer);

            if(authResponse.Customer != null)
            {
                return new OkObjectResult(JToken.FromObject(new RegisterResponseModel()
                {
                    Customer = (CustomerNoPass)authResponse.Customer,
                    AccessToken = authResponse.Token,
                    Expires_In = $"{authResponse.Expiry}h"
                }, new JsonSerializer {
                    
                }));
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = authResponse.Code,
                Message = authResponse.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }


        /// <summary>
        /// Signs a customer in, into their account
        /// </summary>
        /// <param name="credentials">An object of credentails</param>
        /// <returns></returns>
        [HttpPost("login")]
        [ModelValidate]
        public async Task<ActionResult> SignIn([FromBody] LoginModel credentials)
        {
            var authResponse = await customers.SignIn(credentials);

            if (authResponse.Customer != null)
            {
                return new OkObjectResult(new RegisterResponseModel()
                {
                    Customer = (CustomerNoPass)authResponse.Customer,
                    AccessToken = authResponse.Token,
                    Expires_In = $"{authResponse.Expiry}h"
                });
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = authResponse.Code,
                Message = authResponse.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        

        /// <summary>
        /// Updates a customer's credit card numbers
        /// </summary>
        /// <param name="credit_card">new credit-card number</param>
        /// <returns>A Customer object</returns>
        [HttpPut("creditCard")]
        [ModelValidate]
        public async Task<ActionResult> UpdateCreditCard([FromBody]string credit_card)
        {
            int custId = Convert.ToInt32(User.Identity.Name);

            var result = await customers.UpdateAsync(credit_card, custId);

            if (result != null)
            {
                return new OkObjectResult((CustomerNoPass)result);
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = Constants.ErrorCodes.SVR_00.ToString("g"),
                Message = Constants.ErrorMessages.SVR_00,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}