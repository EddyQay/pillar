using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Turing_Back_ED.Models
{
    /// <summary>
    /// Refines a shopping cart product item for output.
    /// It collates all properties from linked entities of the 
    /// ShoppingCart instance, and defines a unified output object
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ShoppingCartProductItem
    {
        public int ItemId { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        [JsonIgnore]
        public Guid CartId { get; set; }
        
        public int ProductId { get; set; }

        //[JsonIgnore]
        public int? CategoryId { get; set; }

        //[JsonIgnore]
        public string Description { get; set; }
        
        public string Attributes { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal SubTotal { get; set; }

        [JsonIgnore]
        public bool BuyNow { get; set; }


        [JsonIgnore]
        public decimal DiscountedPrice { get; set; }

        [JsonIgnore]
        public string Image2 { get; set; }

        [JsonIgnore]
        public string Thumbnail { get; set; }

        [JsonIgnore]
        public short Display { get; set; }

        [JsonIgnore]
        public DateTimeOffset? Added { get; set; }

        [JsonIgnore]
        public DateTimeOffset? Modified { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        /// <summary>
        /// Initializes a new instance of the ShoppingCartProductItem class
        /// </summary>
        public ShoppingCartProductItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the ShoppingCartProductItem class
        /// with an instance of a ShoppingCart class to import data from
        /// </summary>
        /// <param name="cartItem">The ShoppingCart instance to import data from</param>
        public ShoppingCartProductItem(ShoppingCart cartItem)
        {
            Product = cartItem.Product;
            Category = Product.Category;

            ItemId = cartItem.ItemId;
            Name = Product.Name;
            Image = Product.Image;
            CartId = cartItem.CartId;
            ProductId = cartItem.ProductId;
            Attributes = cartItem.Attributes;
            Quantity = cartItem.Quantity;

            Price = ((Product.DiscountedPrice < Product.Price) && Product.DiscountedPrice > 0.0m)
                ? Product.DiscountedPrice
                : Product.Price;

            DiscountedPrice = Product.DiscountedPrice;

            SubTotal = Price * Quantity;

            BuyNow = cartItem.BuyNow;
            CategoryId = Product.CategoryId;
            Description = Product.Description;
            Image2 = Product.Image2;
            Thumbnail = Product.Thumbnail;
            Display = Product.Display;
            Added = cartItem.Added;
            Modified = cartItem.Modified;
        }

        /// <summary>
        /// Imports data from a ShoppingCart instance for refined output
        /// </summary>
        /// <param name="cartItem">The ShoppingCart instance to import data from</param>
        /// <returns>ShoppingCartProductItem</returns>
        public ShoppingCartProductItem From(ShoppingCart cartItem)
        {
            Product = cartItem.Product;
            Category = Product.Category;

            ItemId = cartItem.ItemId;
            Name = Product.Name;
            Image = Product.Image;
            CartId = cartItem.CartId;
            ProductId = cartItem.ProductId;
            Attributes = cartItem.Attributes;
            Quantity = cartItem.Quantity;
            Price = Product.Price;
            DiscountedPrice = Product.DiscountedPrice;

            SubTotal = ((Product.DiscountedPrice < Product.Price) && Product.DiscountedPrice > 0.0m) 
                ? Product.DiscountedPrice * cartItem.Quantity 
                : Product.Price * cartItem.Quantity;

            BuyNow = cartItem.BuyNow;
            CategoryId = Product.CategoryId;
            Description = Product.Description;
            Image2 = Product.Image2;
            Thumbnail = Product.Thumbnail;
            Display = Product.Display;
            Added = cartItem.Added;
            Modified = cartItem.Modified;

            return this;
        }
    }
}
