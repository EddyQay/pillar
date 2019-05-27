using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;
using Turing_Back_ED.Models;
using Moq;
using System.Collections.Generic;
using System;
using Turing_Back_ED.Tests.DAL;
using Turing_Back_ED.Tests.DomainModels;
using Turing_Back_ED.Tests.Controllers;
using Microsoft.AspNetCore.Mvc;
using Turing_Back_ED.Controllers;
using Turing_Back_ED.DAL;
using Turing_Back_ED.DomainModels;

namespace Turing_Back_ED.Tests
{
    [TestFixture]
    public class Tests
    {
        //private ProductStore products;
        private ProductsController productsController;
        
        [SetUp]
        public void Testss()
        {
            //Arrange
            //products = new ProductStore(new Models.TestContext);
            //productsController = new ProductsController(products);

            var repo = new Mock<ProductStore>();
            repo.Setup(p => p.GetAllAsync(new GeneralQueryModel()))
                .ReturnsAsync(GetProductList);

            productsController = new ProductsController(repo.Object, null);
        }

        [Test]
        public void Test1()
        {
            //Act
            var prods = productsController.GetAll(new GeneralQueryModel());

            //Assert
            Assert.IsTrue(prods.Result.Value != null, "List is empty");
            //var newList = prods.Value.OrderBy(a => a.ProductId).Select(a => a);
            //Assert.IsTrue(newList.First().Name.Equals("Product1"),$"Value is {newList.First().Name}");
        }

        private List<Product> GetProductList()
        {
            var list = new List<Product>();
            list.Add(new Product()
            {
                ProductId = 1,
                Name = "Product1",
                Price = 3.4m,
                Added = DateTime.UtcNow,
                Description = "Some cool product from Eddy"
            });

            list.Add(new Product()
            {
                ProductId = 2,
                Name = "Product2",
                Price = 17.2m,
                Added = DateTime.UtcNow,
                Description = "Second product from Eddy"
            });

            return list;
        }
    }
}