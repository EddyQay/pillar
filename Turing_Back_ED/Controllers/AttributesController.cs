using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Turing_Back_ED.DAL;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Middlewares;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;
using Attribute = Turing_Back_ED.Models.Attribute;

namespace Turing_Back_ED.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributesController : ControllerBase
    {
        private readonly AttributeStore attributes;
        readonly ILogger<AttributesController> logger;

        public AttributesController(AttributeStore _attributes, ILogger<AttributesController> _logger)
        {
            attributes = _attributes;
            logger = _logger;
        }
        

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