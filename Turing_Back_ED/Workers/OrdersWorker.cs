﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.Workers
{
    public class OrdersWorker : IStore<Order>
    {
        private readonly DatabaseContext _context;
        private readonly TokenSection tokenSection;
        private readonly TokenManager tokenManager;
        readonly ShoppingCartsWorker shoppingcart;
        readonly CustomerWorker customers;

        /// <summary>
        /// Initiatlizes an instanceof the OrderStore class
        /// </summary>
        /// <param name="context">An instance of the database context class of the application</param>
        /// <param name="_tokenManager">An instance of the TokenManager class</param>
        /// <param name="_customers">An instance of the Customer class</param>
        /// <param name="_tokenSection">An instance of the TokenSection class</param>
        /// <param name="_shoppingcart">An instance of the ShoppingCartStore class</param>
        public OrdersWorker(DatabaseContext context, TokenManager _tokenManager, CustomerWorker _customers,
            IOptions<TokenSection> _tokenSection, ShoppingCartsWorker _shoppingcart)
        {
            _context = context;
            tokenManager = _tokenManager;
            tokenSection = _tokenSection.Value;
            shoppingcart = _shoppingcart;
            customers = _customers;
        }

        /// <summary>
        /// Creates a new order for a customer
        /// </summary>
        /// <param name="entity">An instance of the OrderInputModel 
        /// class used to collect the order information from input</param>
        /// <returns>Id of the new order created</returns>
        public async Task<int> AddAsync(OrderInputModel entity)
        {
            //Get all items or the shopping cart with the given id
            var cartItems =  shoppingcart.FindAllActiveByCartId(entity.CartId);

            if(cartItems?.Count() < 1)
            {
                return 0;
            }

            //create a new order item to the cart items
            var newOrder = new Order
            {
                ShippingId = entity.ShippingId,
                TaxId = entity.TaxId,
                TotalAmount = ShoppingCartProductItems.ComputeTotalAmount(cartItems),
                CreatedOn = DateTime.UtcNow,
                Status = (int)Constants.OrderStatuses.PendingShipment,
                Comments = entity.Comments,
                Reference = new Random(int.MaxValue).Next().ToString(),
                AuthCode = tokenManager.GenerateGenericAuthCode(),
                CustomerId = entity.CustomerId
            };

            //save order
            await _context.Orders.AddAsync(newOrder);
            await SaveChangesAsync();

            //add cart items to ordertails
            await _context.OrderDetails.AddRangeAsync(OrderDetailItems
                .From(newOrder.OrderId, cartItems));
            await SaveChangesAsync();

            await shoppingcart.EmptyCart(entity.CartId);

            //intentionally not awaited, so the process would continue without waiting 
            //for the mail to be sent
            await SendMailAsync((int)newOrder.CustomerId, newOrder.OrderId, newOrder.CreatedOn);

            return newOrder.OrderId;

        }

        /// <summary>
        /// Find an order
        /// </summary>
        /// <param name="Id">order Id</param>
        /// <returns>Order</returns>
        public async Task<Order> FindByIdAsync(int Id)
        {
            return await _context.Orders.FindAsync(Id);
        }

        /// <summary>
        /// Find an order indlucing its order details
        /// </summary>
        /// <param name="Id">ordr id</param>
        /// <returns>Order</returns>
        public async Task<Order> FindByIdVerboseAsync(int Id)
        {
            var orderDetail = await _context.Orders
                .Where(order => order.OrderId == Id)
                .Include(order => order.OrderDetails)
                .SingleOrDefaultAsync();

            return orderDetail;

        }

        /// <summary>
        /// Find all orders made by a customer
        /// </summary>
        /// <param name="customerId">customer's Id</param>
        /// <returns>List of orders</returns>
        public async Task<IEnumerable<Order>> FindByCustomerAsync(int customerId)
        {
            var orders = await _context.Orders
                .Where(order => order.CustomerId == customerId)
                .Include(order => order.OrderDetails)
                .ToListAsync();

            return orders;
        }

        /// <summary>
        /// Saves all changes made in the data context
        /// into the underlying database
        /// </summary>
        /// <returns>number of rows affected</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        } 

        async Task SendMailAsync(int customerId, int orderId, DateTime orderDate)
        {
            var customer = await customers.FindByIdAsync(customerId);
            
            var smtpClient = new SmtpClient()
            {
                //replace with your own provider settings
                Host = "smtp-mail.outlook.com", 
                Port = 587,
                EnableSsl = true,
                
                //use this only when logged into
                //windows with mail account
                //UseDefaultCredentials = true,

                //else uncomment this code
                Credentials = new NetworkCredential("<mailaddress>", "<password>")
            };

            var mailMessage = new MailMessage("no-reply@pillar.com", customer.Email)
            {
                From = new MailAddress("no-reply@pillar.com","Pillar E-Commerce", Encoding.UTF8),
                Subject = "Your Order Has Been Confirmed",
                Body = $"Hello {customer.Name},\n\n" +
                $"your recent order made at Pillar E-Commerce, on " +
                $"{orderDate.ToLongTimeString()}, has been successfully confirmed.\n\n" +
                $"You will recieve a confimation message once your order is shipped. In the meantime," +
                $"you may access details of your order from <a href=\"https://localhost:5001/api/orders/{orderId}\">here</a>" +
                $"\n\nThank you for your order\n\n" +
                $"\n\nThe Pillar E-Commerce Sales Team"
            };

            using (mailMessage)
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            
        }

        #region NOT IMPLEMENTED

        public Task<IEnumerable<Order>> GetAllAsync(GeneralQueryModel criteria)
        {
            throw new NotImplementedException();
        }  

        public Task<Order> AddAsync(Order entity)
        {
            throw new NotImplementedException();
        }

        public Task<Order> UpdateAsync(Order entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Order entity)
        {
            throw new NotImplementedException();
        }

        Task<Order> IStore<Order>.FindByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> FindAllAsync(SearchModel criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> FindByConditionAsync(Expression<Func<Order, bool>> expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
