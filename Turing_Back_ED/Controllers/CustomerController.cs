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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerStore customers;
        readonly ILogger<CustomerController> logger;

        public CustomerController(CustomerStore _customers, ILogger<CustomerController> _logger)
        {
            customers = _customers;
            logger = _logger;
        }
        
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