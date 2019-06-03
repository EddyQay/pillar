using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Turing_Back_ED.Workers;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Middlewares;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.Controllers
{

    /// <summary>
    /// Facilitates shipping information access
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly ShippingsWorker shippings;

        public ShippingController(ShippingsWorker _shippings)
        {
            shippings = _shippings;
        }
        
        /// <summary>
        /// Finds a particular shipping information
        /// </summary>
        /// <param name="Id">Shipping id</param>
        /// <returns>A Shipping object</returns>
        [HttpGet("{id}")]
        [ModelValidate]
        public async Task<ActionResult> FindShipping(int Id)
        {
            var shipping = await shippings.FindByIdAsync(Id);

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


        /// <summary>
        /// Finds a particular shipping region
        /// </summary>
        /// <param name="region_Id">Shipping region Id</param>
        /// <returns>A Shipping region object</returns>
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
                Message = string.Format(Constants.ErrorMessages.ERR_01, "Shipping Region information"),
                Status = StatusCodes.Status400BadRequest
            });
        }

        /// <summary>
        /// Gets all shipping information
        /// </summary>
        /// <param name="filter">An object of filtering options</param>
        /// <returns>An array of Shipping objects</returns>
        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAll(GeneralQueryModel filter)
        {
            if (filter == null)
                filter = new GeneralQueryModel();

            var query = await shippings.GetAllAsync(filter);

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


        /// <summary>
        /// Gets all shipping regions
        /// </summary>
        /// <param name="filter">An object of filtering objects</param>
        /// <returns>An array of shipping regions</returns>
        [HttpGet("regions")]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAllRegions(GeneralQueryModel filter)
        {

            if (filter == null)
                filter = new GeneralQueryModel();

            var query = await shippings.GetAllShippingRegions(filter);

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