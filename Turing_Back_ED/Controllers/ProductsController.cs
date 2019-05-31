using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Turing_Back_ED.Workers;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;
using Turing_Back_ED.Middlewares;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Session;

namespace Turing_Back_ED.Controllers
{
    /// <summary>
    /// Facilitates product creation and manipulation 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsWorker products;

        public ProductsController(ProductsWorker _products)
        {
            products = _products;
        }


        /// <summary>
        /// Gets all products there is
        /// </summary>
        /// <param name="filter">An object of filtering options</param>
        /// <returns>An array of Products</returns>
        [HttpGet]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll(GeneralQueryModel filter)
        {
            if(filter == null)
                filter = new GeneralQueryModel();
            
            var query = await products.GetAllAsync(filter);

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
        /// Finds a particular product
        /// </summary>
        /// <param name="id">Id of prodcut</param>
        /// <returns>A Product object</returns>
        [HttpGet("{id}")]
        [ModelValidate]
        public async Task<ActionResult<IEnumerable<Product>>> Find(int id)
        {
            return new OkObjectResult(await products.FindByIdAsync(id));
        }


        /// <summary>
        /// Get detailed information in a particular product
        /// </summary>
        /// <param name="Id">Id of product</param>
        /// <returns>An object of product details</returns>
        [HttpGet("{id}/details")]
        [ModelValidate]
        public async Task<ActionResult> FindDetails(int Id)
        {
            return new OkObjectResult(await products.FindById_D(Id));
        }


        /// <summary>
        /// Finds products based on defined set of options
        /// </summary>
        /// <param name="filter">An object of filtering options</param>
        /// <returns>An array of products</returns>
        [HttpGet("search")]
        [ModelValidate]
        public async Task<ActionResult<IEnumerable<Product>>> FindBySearch(SearchModel filter)
        {
            var searchResult = await products.FindAllAsync(filter);

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


        /// <summary>
        /// Finds all products that belong to a particular category
        /// </summary>
        /// <param name="Id">Id of category</param>
        /// <param name="filter">An object of filtering options</param>
        /// <returns>An Array of products</returns>
        [HttpGet("inCategory/{id}")]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult<IEnumerable<Product>>> InCategory(int Id, GeneralQueryModel filter)
        {
            if (!ModelState.IsValid && Id < 1)
            {

                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), 
                    new Newtonsoft.Json.JsonSerializer{
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

                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), 
                    new Newtonsoft.Json.JsonSerializer {
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

            if (filter == null)
                filter = new GeneralQueryModel();

            var searchResult = await products.FindInCategory(Id, filter);

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


        /// <summary>
        /// Finds all products that belong to a particular department
        /// </summary>
        /// <param name="Id">Id of department</param>
        /// <param name="filter">An object of filtering options</param>
        /// <returns>An array of products</returns>
        [HttpGet("inDepartment/{id}")]
        [ModelValidate(allowNull: true)]
        public async Task<ActionResult<IEnumerable<Product>>> InDepartment(int Id, GeneralQueryModel filter)
        {
            if (!ModelState.IsValid && Id < 1)
            {
                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), 
                    new Newtonsoft.Json.JsonSerializer{
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
                var errors = JToken.FromObject(ModelState.Values.SelectMany(a => a.Errors), 
                    new Newtonsoft.Json.JsonSerializer{
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

            if (filter == null)
                filter = new GeneralQueryModel();

            var searchResult = await products.FindInDepartment(Id, filter);

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


        /// <summary>
        /// Finds all locations for a particular product 
        /// </summary>
        /// <param name="Id">Id of product</param>
        /// <returns>An array of product locations</returns>
        [HttpGet("{id}/locations")]
        [ModelValidate]
        public async Task<ActionResult<ProductLocation>> Locations(int Id)
        {
            var searchResult = await products.FindLocations(Id);
            return new OkObjectResult(searchResult);
        }


        /// <summary>
        /// Gets all reviews on a particular product
        /// </summary>
        /// <param name="Id">Id of product</param>
        /// <returns></returns>
        [HttpGet("{id}/reviews")]
        [ModelValidate]
        public async Task<ActionResult<IEnumerable<Review_>>> GetReviews(int Id)
        {
            var searchResult = await products.GetReviews(Id);
            return new OkObjectResult(searchResult);
        }

        /// <summary>
        /// Add a review on a particular product
        /// </summary>
        /// <param name="Id">Id of product</param>
        /// <param name="reviewItem">review object of review details</param>
        /// <returns>No content</returns>
        [Authorize]
        [HttpPost("{id}/reviews")]
        [ModelValidate]
        public async Task<ActionResult<IEnumerable<Review_>>> AddReview(int Id, ReviewModel reviewItem)
        {
            var review = new Review_
            {
                ProductId = Id,
                Review = reviewItem.Review,
                Rating = reviewItem.Rating
            };

            products.AddReview(review, HttpContext);
            await products.SaveChangesAsync();
            return new OkResult();
        }
    }
}