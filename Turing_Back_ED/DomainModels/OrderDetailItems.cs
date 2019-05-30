using System;
using System.Collections.Generic;

namespace Turing_Back_ED.Models
{
    public static class OrderDetailItems
    {
        static List<OrderDetail> orderDetails;
        static readonly List<OrderDetailItem> orderDetailItems;

        /// <summary>
        /// Initializes a new instance of the OrderDetailItems class
        /// </summary>
        static OrderDetailItems()
        {
            orderDetails = new List<OrderDetail>();
            orderDetailItems = new List<OrderDetailItem>();
        }

        /// <summary>
        /// Imports data from a ShoppingCartProductItem list instance for refined output
        /// </summary>
        /// <param name="orderId">id of the new order</param>
        /// <param name="cartItemList">The ShoppingCartProductItem list, to import data from</param>
        /// <returns>IEnumerable of OrderDetail</returns>
        public static IEnumerable<OrderDetail> From(int orderId, IEnumerable<ShoppingCartProductItem> cartItemList)
        {
            //orderDetails = new List<OrderDetail>();
            foreach (var cartItem in cartItemList)
            {
                orderDetails.Add(new OrderDetail().From(orderId, cartItem));
            }

            return orderDetails;
        }

        /// <summary>
        /// Imports data from an OrderItem list instance for refined output
        /// </summary>
        /// <param name="orderDetails">The OrderItem list, to import data from</param>
        /// <returns>IEnumerable of OrderDetail</returns>
        public static IEnumerable<OrderDetailItem> From(IEnumerable<OrderDetail> orderDetails)
        {
            foreach (var orderDetail in orderDetails)
            {
                orderDetailItems.Add(new OrderDetailItem().From(orderDetail));
            }

            return orderDetailItems;
        }
    }
}
