using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
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
    /// <summary>
    /// Controls shopping carts
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly ShoppingCartStore shoppingCart;

        public ShoppingCartController(ShoppingCartStore _shoppingCart)
        {
            shoppingCart = _shoppingCart;
        }

        /// <summary>
        /// Generates a unique id for a new shoppoing cart
        /// </summary>
        /// <returns>UUID string property</returns>
        [HttpGet("uniqueId")]
        public ActionResult GetUniqueId()
        {
            return new OkObjectResult(new
            {
                cartId = shoppingCart.GenerateUniqueId()
            });
        }


        /// <summary>
        /// Adds a new product or item to a shopping cart
        /// </summary>
        /// <param name="cartItem">An object representing a product to add</param>
        /// <returns>ShoppingCart object</returns>
        [HttpPost]
        [ModelValidate]
        public async Task<ActionResult> AddToCart(ShoppingCartForInput cartItem)
        {
            var addedItem = await shoppingCart.AddAsync((ShoppingCart)cartItem);

            if (addedItem != null)
            {
                return new OkObjectResult(new ShoppingCartProductItem().From(addedItem));
            }

            return new InternalServerErrorResultObject(new ErrorRequestModel()
            {
                Code = nameof(Constants.ErrorMessages.ERR_02),
                Message = Constants.ErrorMessages.ERR_02,
                Status = StatusCodes.Status500InternalServerError
            });
        }


        /// <summary>
        /// Get all product in a particular shopping cart
        /// </summary>
        /// <param name="cartId">shopping cart Id</param>
        /// <returns>An array of shopping cart items</returns>
        [HttpGet("{cartId}")]
        [ModelValidate]
        public async Task<ActionResult> ProductsInCart(Guid cartId)
        {
            GeneralQueryModel model = new GeneralQueryModel();

            var query = await shoppingCart.FindAllByCartIdAsync(cartId, model);

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
        /// Updates a particular item in a shopping cart
        /// </summary>
        /// <param name="Id">Item Id</param>
        /// <param name="quantity">New quantity of the item to order</param>
        /// <returns>Shopping cart item object</returns>
        [HttpPut("item/{Id}")]
        [ModelValidate]
        public async Task<ActionResult> UpdateItemInCart(int Id, [FromBody]int quantity)
        {
            var updatedItem = await shoppingCart.UpdateAsync(Id, quantity);

            if (updatedItem != null)
                return new OkObjectResult(new ShoppingCartProductItem().From(updatedItem));
            else
                return new InternalServerErrorResultObject(new ErrorRequestModel()
                {
                    Code = nameof(Constants.ErrorMessages.ERR_02),
                    Message = Constants.ErrorMessages.ERR_02,
                    Status = StatusCodes.Status500InternalServerError
                });
        }
        

        /// <summary>
        /// Removes of all items form a shooping cart
        /// </summary>
        /// <param name="cartId">shopping cart Id</param>
        /// <returns>An empty array(on success) or a shopping cart object (on failure)</returns>
        [HttpDelete("{cartId}")]
        [ModelValidate]
        public async Task<ActionResult> EmptyCart(Guid cartId)
        {
            var emptyItem = await shoppingCart.EmptyCart(cartId);

            if (emptyItem != null)
            {
                return new OkObjectResult(emptyItem);
            }

            return new OkObjectResult(new List<ShoppingCart>());
        }

        /// <summary>
        /// Suspends an item from a shopping cart.
        /// This item will not be added to any order until it
        /// is re-activated
        /// </summary>
        /// <param name="Id">Item Id</param>
        /// <returns>No content</returns>
        [HttpPut("saveForLater/{Id}")]
        [ModelValidate]
        public async Task<ActionResult> SaveItemForLater(int Id)
        {
            var rowsAffected = await shoppingCart.SaveItemForLater(Id);

            if (rowsAffected > 0)
            {
                return new OkResult();
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = nameof(Constants.ErrorMessages.ERR_01),
                Message = string.Format(Constants.ErrorMessages.ERR_01, "item in cart"),
                Status = StatusCodes.Status400BadRequest
            });
        }

        /// <summary>
        /// Moves an item back into the shopping cart after saving for later
        /// </summary>
        /// <param name="itemId">item Id</param>
        /// <returns>No Content</returns>
        [HttpPut("moveToCart/{itemId}")]
        [ModelValidate]
        public async Task<ActionResult> MoveItemToCart(int itemId)
        {
            var rowsAffected = await shoppingCart.MoveItemToCart(itemId);

            if (rowsAffected > 0)
            {
                return new OkResult();
            }

            return new InternalServerErrorResultObject(new ErrorRequestModel()
            {
                Code = nameof(Constants.ErrorMessages.ERR_01),
                Message = string.Format(Constants.ErrorMessages.ERR_01, "item in cart"),
                Status = StatusCodes.Status400BadRequest
            });
        }


        /// <summary>
        /// Computes the total cost of item in a shopping cart
        /// </summary>
        /// <param name="cartId">Cart Id</param>
        /// <returns>An object with a single property - totalItems</returns>
        [HttpGet("totalItems/{cartId}")]
        [ModelValidate]
        public async Task<ActionResult> CountTotalItemsInCart(Guid cartId)
        {
            var totalItems = await shoppingCart.GetTotalItemCountForCart(cartId);

            return new OkObjectResult(new
            {
                totalItems
            });
        }


        /// <summary>
        /// Gets all items that have been saved from the shopping cart 
        /// for a later time
        /// </summary>
        /// <param name="cartId">Shopping cart Id</param>
        /// <returns>An array products</returns>
        [HttpGet("saved/{cartId}")]
        [ModelValidate]
        public async Task<ActionResult> GetSavedItems(Guid cartId)
        {
            var savedItems = await shoppingCart.GetSavedItemsInCart(cartId);

            return new OkObjectResult(new SearchResponseModel
            {
                Count = savedItems.Count(),
                Rows = savedItems
            });
        }


        /// <summary>
        /// Permanently removes an item form a shopping cart
        /// </summary>
        /// <param name="Id">Item Id</param>
        /// <returns>No content</returns>
        [HttpDelete("item/{id}")]
        [ModelValidate]
        public async Task<ActionResult> RemoveFromCart(int Id)
        {
            var cartItem = await shoppingCart.FindByIdAsync(Id);

            if (cartItem != null)
            {
                var rowsAffected = await shoppingCart.Remove(Id);

                if (rowsAffected > 0)
                    return new OkResult();

                return new InternalServerErrorResultObject(new ErrorRequestModel()
                {
                    Code = nameof(Constants.ErrorMessages.ERR_02),
                    Message = Constants.ErrorMessages.ERR_02,
                    Status = StatusCodes.Status500InternalServerError
                });
            }

            return new BadRequestObjectResult(new ErrorRequestModel()
            {
                Code = nameof(Constants.ErrorMessages.ERR_01),
                Message = string.Format(Constants.ErrorMessages.ERR_01, "shopping cart item"),
                Status = StatusCodes.Status400BadRequest
            });
        }
        
    }
}