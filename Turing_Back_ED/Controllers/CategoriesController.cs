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
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryStore categories;
        readonly ILogger<CategoriesController> logger;

        public CategoriesController(CategoryStore _categories, ILogger<CategoriesController> _logger)
        {
            categories = _categories;
            logger = _logger;
        }


        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetAll(CategoryQueryModel model)
        {
            if (model == null)
                model = new CategoryQueryModel();

            if(!string.IsNullOrWhiteSpace(model.Order))
            {
                var (valid, errorType, message) = categories.IsSortOrderValid(model.Order);

                if (!valid)
                {
                    return new BadRequestObjectResult(new BadRequestModel()
                    {
                        Code = errorType.ToString("g"),
                        Message = message,
                        Field = nameof(model.Order),
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            }

            var query = await categories.GetAllAsync(model);

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
        

        [HttpGet("{id}")]
        [ModelValidate]
        public async Task<ActionResult> FindCategory(int id)
        {
            var result = await categories.FindByIdAsync(id);

            if (result != null)
            {
                return new OkObjectResult(result);
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = Constants.ErrorCodes.CAT_01.ToString("g"),
                Message = Constants.ErrorMessages.CAT_01,
                Status = StatusCodes.Status400BadRequest
            });
        }
        

        [HttpGet("inProduct/{product_Id}")]
        [ModelValidate(allowNull:true)]
        public async Task<ActionResult> GetCategoriesInProduct(int product_Id, GeneralQueryModel model)
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

            var attributeValues = await categories.GetProductCategories(product_Id, model);

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


        [HttpGet("inDepartment/{department_Id}")]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult> GetCategoriesInDepartment(int department_Id, GeneralQueryModel model)
        {
            if (!ModelState.IsValid && department_Id < 1)
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
            else if (department_Id < 1)
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

            var attributeValues = await categories.GetDepartmentCategories(department_Id, model);

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