using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Turing_Back_ED.Models
{
    /// <summary>
    /// Refines a list of shopping cart product items for output.
    /// It collates all properties from linked entities of the 
    /// ShoppingCart instances, and defines a unified enumeration
    /// of output objects
    /// </summary>
    /// <Authur>Eddy Ayi</Authur>
    public static class ShoppingCartProductItems
    {
        static List<ShoppingCartProductItem> outputProductItems;

        /// <summary>
        /// Initializes a new instance of the ShoppingCartProductItems class
        /// </summary>
        static ShoppingCartProductItems()
        {
            outputProductItems = new List<ShoppingCartProductItem>();
        }

        /// <summary>
        /// Imports data from a ShoppingCart list instance for refined output
        /// </summary>
        /// <param name="cartItemList">The ShoppingCart list, to import data from</param>
        /// <returns> Enumerable of ShoppingCartProductItem</returns>
        public static IEnumerable<ShoppingCartProductItem> From(IEnumerable<ShoppingCart> cartItemList)
        {
            outputProductItems = new List<ShoppingCartProductItem>();
            foreach (var cartItem in cartItemList)
            {
                outputProductItems.Add(new ShoppingCartProductItem().From(cartItem));
            }

            return outputProductItems;
        }

        /// <summary>
        /// Imports data from a ShoppingCart list instance for refined output
        /// </summary>
        /// <param name="cartItemList">The ShoppingCart queryable list, to import data from</param>
        /// <returns> Enumerable of ShoppingCartProductItem</returns>
        public static IEnumerable<ShoppingCartProductItem> From(IQueryable<ShoppingCart> cartItemList)
        {
            outputProductItems = new List<ShoppingCartProductItem>();
            foreach (var cartItem in cartItemList)
            {
                outputProductItems.Add(new ShoppingCartProductItem().From(cartItem));
            }

            return outputProductItems;
        }

        /// <summary>
        /// Computes the the total cost of items in a
        /// shopping cart using an IEnumerable of ShoppingCartProductItems
        /// </summary>
        /// <param name="cartItemList">An enumerable of ShoppingCartProductItems</param>
        /// <returns>decimal</returns>
        public static decimal ComputeTotalAmount(IEnumerable<ShoppingCartProductItem> cartItemList)
        {
            decimal totalAmount = 0.00m;
            foreach (var cartItem in cartItemList)
            {
                totalAmount += cartItem.SubTotal;
            }

            return decimal.Round(totalAmount, 4);
        }

        /// <summary>
        /// Computes the the total cost of items in a
        /// shopping cart using an IQueryable of ShoppingCartProductItems
        /// </summary>
        /// <param name="cartItemList">Queryable list  ShoppingCartProductItem</param>
        /// <returns>decimal</returns>
        public static decimal ComputeTotalAmount(IQueryable<ShoppingCartProductItem> cartItemList)
        {
            return decimal.Round(cartItemList.Select(a => a.SubTotal).Sum(), 4);

            #region OLD IMPLEMENTATION
            //decimal totalAmount = 0.00m;
            //foreach (var cartItem in cartItemList)
            //{
            //    totalAmount += cartItem.SubTotal;
            //}

            //return totalAmount;
            #endregion
        }
    }
}
