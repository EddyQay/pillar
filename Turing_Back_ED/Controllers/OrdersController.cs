﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    /// Facilitates The creation and manipulation of orders
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderStore orders;

        public OrdersController(OrderStore _orders)
        {
            orders = _orders;
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [ModelValidate]
        public async Task<ActionResult> CreateOrder(OrderInputModel order)
        {
            order.CustomerId = Convert.ToInt32(User.Identity.Name);
            var orderId = await orders.AddAsync(order);

            if (orderId > 0)
            {
                return new OkObjectResult(new {
                    OrderId = orderId
                });
            }

            return new InternalServerErrorResultObject(new ErrorRequestModel()
            {
                Code = nameof(Constants.ErrorMessages.ERR_02),
                Message = Constants.ErrorMessages.ERR_02,
                Status = StatusCodes.Status500InternalServerError
            });
        }

        /// <summary>
        /// Finds an existing order by its orderId
        /// </summary>
        /// <param name="orderId">Id of the order to find</param>
        /// <returns>Order</returns>
        [HttpGet("{orderId}")]
        [ModelValidate]
        public async Task<ActionResult> FindOrderLong(int orderId)
        {
            var order = await orders.FindByIdVerboseAsync(orderId);

            if (order != null)
                return new OkObjectResult(order);
            else
                return new BadRequestObjectResult(new ErrorRequestModel()
                {
                    Code = nameof(Constants.ErrorMessages.ERR_01),
                    Message = Constants.ErrorMessages.ERR_01,
                    Status = StatusCodes.Status400BadRequest
                });
        }

        /// <summary>
        /// Retrieves brief details of an order
        /// </summary>
        /// <param name="Id">order Id</param>
        /// <returns>Order</returns>
        [HttpGet("shortDetail/{Id}")]
        [ModelValidate]
        public async Task<ActionResult> FindOrderShort(int Id)
        {
            var order = await orders.FindByIdAsync(Id);

            if (order != null)
                return new OkObjectResult(order);
            else
                return new BadRequestObjectResult(new ErrorRequestModel()
                {
                    Code = nameof(Constants.ErrorMessages.ERR_01),
                    Message = Constants.ErrorMessages.ERR_01,
                    Status = StatusCodes.Status400BadRequest
                });
        }

        /// <summary>
        /// Returns a list of orders made by a customer
        /// </summary>
        /// <returns>List of orders</returns>
        [HttpGet("inCustomer")]
        [ModelValidate]
        public async Task<ActionResult> OrdersByCustomer()
        {
            int customerId = Convert.ToInt32(User.Identity.Name);
            var query = await orders.FindByCustomerAsync(customerId);

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