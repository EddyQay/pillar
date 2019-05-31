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
    /// Manipulates a particular customer
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerWorker customers;

        public CustomerController(CustomerWorker _customers)
        {
            customers = _customers;
        }
        
        /// <summary>
        /// Finds a particular customer
        /// </summary>
        /// <returns>A Customer object</returns>
        [HttpGet]
        public async Task<ActionResult> FindCustomer()
        {
            int custId = Convert.ToInt32(User.Identity.Name);
            var customer = await customers.FindByIdAsync(custId);

            if (customer != null)
            {
                return new OkObjectResult((CustomerNoPass)customer);
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = Constants.ErrorCodes.USR_00.ToString("g"),
                Message = Constants.ErrorMessages.USR_00,
                Status = StatusCodes.Status400BadRequest
            });
        }

        /// <summary>
        /// Updates a partuclar customer's information
        /// </summary>
        /// <param name="customer">A customer object with updated information</param>
        /// <returns>A Customer object</returns>
        [HttpPut]
        public async Task<ActionResult> UpdateCustomer(CustomerForUpdate customer)
        {
            customer.CustomerId= Convert.ToInt32(User.Identity.Name);

            var result = await customers.UpdateAsync((Customer)customer);
            if (result != null)
            {
                return new OkObjectResult((CustomerNoPass)result);
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = Constants.ErrorCodes.USR_00.ToString("g"),
                Message = Constants.ErrorMessages.USR_00,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}