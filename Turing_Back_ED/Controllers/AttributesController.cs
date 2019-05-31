using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Turing_Back_ED.Workers;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Middlewares;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;
using Attribute = Turing_Back_ED.Models.Attribute;

namespace Turing_Back_ED.Controllers
{
    /// <summary>
    /// Facilitates the creation and manipulation of attributes
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AttributesController : ControllerBase
    {
        private readonly AttributesWorker attributes;

        public AttributesController(AttributesWorker _attributes)
        {
            attributes = _attributes;
        }
        
        /// <summary>
        /// Finds an attributes using its Id
        /// </summary>
        /// <param name="id">attribute id</param>
        /// <returns>an attribute  object</returns>
        [HttpGet("{id}")]
        [ModelValidate]
        public async Task<ActionResult> FindAttribute(int id)
        {
            var attribute = await attributes.FindByIdAsync(id);

            if (attribute != null)
            {
                return new OkObjectResult(attribute);
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = Constants.ErrorCodes.ERR_01.ToString("g"),
                Message = string.Format(Constants.ErrorMessages.ERR_01, nameof(Attribute).ToLower()),
                Status = StatusCodes.Status400BadRequest
            });
        }

        /// <summary>
        /// Retrieves all attributes
        /// </summary>
        /// <param name="model">An instance of the GeneralQueryModel class</param>
        /// <returns>A list of attributes</returns>
        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAll(GeneralQueryModel model)
        {
            if (model == null)
                model = new GeneralQueryModel();

            var query = await attributes.GetAllAsync(model);

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
        /// Gets all values for an attribute
        /// </summary>
        /// <param name="attribute_Id">Id of the attribute</param>
        /// <param name="model">An instance of the GeneralQueryModel class</param>
        /// <returns>A list of attribute values</returns>
        [HttpGet("values/{attribute_Id}")]
        [ModelValidate]
        public async Task<ActionResult> GetValues(int attribute_Id, GeneralQueryModel model)
        {
            if (!ModelState.IsValid && attribute_Id < 1)
            {
                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), new JsonSerializer
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                });

                return new BadRequestObjectResult(new BadRequestModel
                {
                    Code = $"PRM_01",
                    Status = StatusCodes.Status400BadRequest,
                    Message = Constants.BadRequestMessage,
                    Errors = errors

                });
            }
            else if (attribute_Id < 1)
            {
                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), new JsonSerializer
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                });

                return new BadRequestObjectResult(new BadRequestModel
                {
                    Code = $"PRM_01",
                    Status = StatusCodes.Status400BadRequest,
                    Message = Constants.BadRequestMessage,
                    Errors = errors
                });
            }

            if (model == null)
                model = new GeneralQueryModel();

            var values = await attributes.GetAttributeValues(attribute_Id, model);

            if (values != null)
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = values.Count(),
                    Rows = values
                });
            else
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = 0,
                    Rows = values
                });
        }


        /// <summary>
        /// Gets attributes of a particular product
        /// </summary>
        /// <param name="product_Id">Id of the product</param>
        /// <param name="model">An instance of the GeneralQueryModel class</param>
        /// <returns>A list of attributes</returns>
        [HttpGet("inProduct/{product_Id}")]
        [ModelValidate]
        public async Task<ActionResult> GetAttibutesInProduct(int product_Id, GeneralQueryModel model)
        {
            if (!ModelState.IsValid && product_Id < 1)
            {
                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                return new BadRequestObjectResult(new BadRequestModel
                {
                    Code = $"PRM_01",
                    Status = StatusCodes.Status400BadRequest,
                    Message = Constants.BadRequestMessage,
                    Errors = errors

                });
            }
            else if (product_Id < 1)
            {
                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                return new BadRequestObjectResult(new BadRequestModel
                {
                    Code = $"PRM_01",
                    Status = StatusCodes.Status400BadRequest,
                    Message = Constants.BadRequestMessage,
                    Errors = errors
                });
            }

            if (model == null)
                model = new GeneralQueryModel();

            var attributeValues = await attributes.GetProductAttributes(product_Id, model);

            if (attributeValues != null)
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = attributeValues.Count(),
                    Rows = attributeValues
                });
            else
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = 0,
                    Rows = attributeValues
                });
        }
    }
}