using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Middlewares;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.Controllers
{
    /// <summary>
    /// Handles tax related information
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TaxController : ControllerBase
    {
        private readonly TaxStore taxes;

        public TaxController(TaxStore _taxes)
        {
            taxes = _taxes;
        }
        

        /// <summary>
        /// Find information on a particular tax entry
        /// </summary>
        /// <param name="id">Tax Id</param>
        /// <returns>A tax object</returns>
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

        /// <summary>
        /// Gets all tax entries
        /// </summary>
        /// <param name="filter">An object of filtering options</param>
        /// <returns>An array of tax objects</returns>
        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAll(GeneralQueryModel filter)
        {
            if (filter == null)
                filter = new GeneralQueryModel();

            var query = await taxes.GetAllAsync(filter);

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