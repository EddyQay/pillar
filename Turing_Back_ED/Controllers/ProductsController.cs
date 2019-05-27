using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Turing_Back_ED.DAL;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;
using Turing_Back_ED.Middlewares;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Session;

namespace Turing_Back_ED.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductStore products;
        readonly ILogger<ProductsController> logger;

        public ProductsController(ProductStore _products, ILogger<ProductsController> _logger)
        {
            products = _products;
            logger = _logger;
        }

        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll(GeneralQueryModel model)
        {
            if(model == null)
                model = new GeneralQueryModel();
            
            var query = await products.GetAllAsync(model);

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
        public async Task<ActionResult<IEnumerable<Product>>> Find(int id)
        {
            return new OkObjectResult(await products.FindByIdAsync(id));
        }

        [HttpGet("{id}/details")]
        [ModelValidate]
        public async Task<ActionResult> FindDetails(int id)
        {
            return new OkObjectResult(await products.FindById_D(id));
        }

        [HttpGet("search")]
        [ModelValidate]
        public async Task<ActionResult<IEnumerable<Product>>> FindBySearch(SearchModel model)
        {
            var searchResult = await products.FindAllAsync(model);

            if (searchResult != null)
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = searchResult.Count(),
                    Rows = searchResult
                });
            else
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = 0,
                    Rows = searchResult
                });
        }

        [HttpGet("inCategory/{id}")]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult<IEnumerable<Product>>> InCategory(int Id, GeneralQueryModel model)
        {
            if (!ModelState.IsValid && Id < 1)
            {
                //List<JProperty> errors = new List<JProperty>();
                //ModelState.Values.SelectMany(a => a.Errors).ToList().ForEach(delegate (ModelError modelError) {
                //    errors.Add((JProperty)JToken.Parse($"message:{ modelError.ErrorMessage}"));
                //});

                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), new Newtonsoft.Json.JsonSerializer
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
            else if (Id < 1)
            {
                //List<JProperty> errors = new List<JProperty>();
                //ModelState.Values.SelectMany(a => a.Errors).ToList().ForEach(delegate (ModelError modelError) {
                //    errors.Add((JProperty)JToken..Parse($"message:{ modelError.ErrorMessage}"));
                //});

                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), new Newtonsoft.Json.JsonSerializer {
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

            var searchResult = await products.FindInCategory(Id, model);

            if (searchResult != null)
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = searchResult.Count(),
                    Rows = searchResult
                });
            else
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = 0,
                    Rows = searchResult
                });
        }

        [HttpGet("inDepartment/{id}")]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult<IEnumerable<Product>>> InDepartment(int Id, GeneralQueryModel model)
        {
            if (!ModelState.IsValid && Id < 1)
            {
                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), new Newtonsoft.Json.JsonSerializer
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
            else if (Id < 1)
            {
                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), new Newtonsoft.Json.JsonSerializer
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

            var searchResult = await products.FindInDepartment(Id, model);

            if (searchResult != null)
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = searchResult.Count(),
                    Rows = searchResult
                });
            else
                return new OkObjectResult(new SearchResponseModel
                {
                    Count = 0,
                    Rows = searchResult
                });
        }

        [HttpGet("{id}/locations")]
        [ModelValidate]
        public async Task<ActionResult<ProductLocation>> Locations(int Id)
        {
            var searchResult = await products.FindLocations(Id);
            return new OkObjectResult(searchResult);
            //var searchResult = 
            //if (searchResult != null)
            //    return new OkObjectResult(searchResult);
            //else
            //    return new OkObjectResult(new ProductLocationsModel
            //    {
            //        Count = 0,
            //        Rows = searchResult
            //    });
        }

        [HttpGet("{id}/reviews")]
        [ModelValidate]
        public async Task<ActionResult<IEnumerable<Review_>>> GetReviews(int Id)
        {
            var searchResult = await products.GetReviews(Id);
            return new OkObjectResult(searchResult);
            //var searchResult = 
            //if (searchResult != null)
            //    return new OkObjectResult(searchResult);
            //else
            //    return new OkObjectResult(new ProductLocationsModel
            //    {
            //        Count = 0,
            //        Rows = searchResult
            //    });
        }

        [Authorize]
        [HttpPost("{id}/reviews")]
        [ModelValidate]
        public async Task<ActionResult<IEnumerable<Review_>>> AddReview(int id, ReviewModel model)
        {
            var review = new Review_
            {
                ProductId = id,
                Review = model.Review,
                Rating = model.Rating
            };

            products.AddReview(review, HttpContext);
            await products.SaveChangesAsync();
            return new OkResult();
        }
    }
}