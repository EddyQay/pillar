using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Turing_Back_ED.DAL;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Middlewares;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly ShippingStore shippings;
        readonly ILogger<ShippingController> logger;

        public ShippingController(ShippingStore _shippings, ILogger<ShippingController> _logger)
        {
            shippings = _shippings;
            logger = _logger;
        }
        

        [HttpGet("{id}")]
        [ModelValidate]
        public async Task<ActionResult> FindShipping(int id)
        {
            var shipping = await shippings.FindByIdAsync(id);

            if (shipping != null)
            {
                return new OkObjectResult(shipping);
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = Constants.ErrorCodes.ERR_01.ToString("g"),
                Message = string.Format(Constants.ErrorMessages.ERR_01, nameof(Shipping).ToLower() + "information"),
                Status = StatusCodes.Status400BadRequest
            });
        }

        [HttpGet("regions/{region_Id}")]
        [ModelValidate]
        public async Task<ActionResult> FindRegionById(int region_Id)
        {
            var shippingRegion = await shippings.FindShippingRegionById(region_Id);

            if (shippingRegion != null)
            {
                return new OkObjectResult(shippingRegion);
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = Constants.ErrorCodes.ERR_01.ToString("g"),
                Message = string.Format(Constants.ErrorMessages.ERR_01, nameof(ShippingRegion).ToLower() + "information"),
                Status = StatusCodes.Status400BadRequest
            });
        }

        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAll(GeneralQueryModel model)
        {
            if (model == null)
                model = new GeneralQueryModel();

            var query = await shippings.GetAllAsync(model);

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

        [HttpGet("regions")]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAllRegions(GeneralQueryModel model)
        {

            if (model == null)
                model = new GeneralQueryModel();

            var query = await shippings.GetAllShippingRegions(model);

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