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
    [Route("api/[controller]")]
    [ApiController]
    public class TaxController : ControllerBase
    {
        private readonly TaxStore taxes;
        readonly ILogger<TaxController> logger;

        public TaxController(TaxStore _taxes, ILogger<TaxController> _logger)
        {
            taxes = _taxes;
            logger = _logger;
        }
        

        [HttpGet("{id}")]
        [ModelValidate]
        public async Task<ActionResult> FindTax(int id)
        {
            var tax = await taxes.FindByIdAsync(id);

            if (tax != null)
            {
                return new OkObjectResult(tax);
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = Constants.ErrorCodes.TAX_01.ToString("g"),
                Message = Constants.ErrorMessages.TAX_01,
                Status = StatusCodes.Status400BadRequest
            });
        }

        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAll(GeneralQueryModel model)
        {
            if (model == null)
                model = new GeneralQueryModel();

            var query = await taxes.GetAllAsync(model);

            if (query != null)
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = query.Count(),
                    Rows = query
                });
            else
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = 0,
                    Rows = query
                });
        }
    }
}