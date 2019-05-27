using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Turing_Back_ED.Models;
using Turing_Back_ED.Tests.DAL;

namespace Turing_Back_ED.Tests.Controllers
{
    [ApiController]
    public class ProductsTestController : ControllerBase
    {
        private readonly IStoreTest<Product> products;

        public ProductsTestController(IStoreTest<Product> _products)
        {
            products = _products;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAll()
        {
            return new OkObjectResult(products.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Product>> Find(int id)
        {
            return new OkObjectResult(products.FindById(id));
        }
    }
}