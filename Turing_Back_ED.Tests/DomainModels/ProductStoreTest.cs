using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turing_Back_ED.Models;
using Turing_Back_ED.Tests.DAL;
using Turing_Back_ED.Tests.Models;

namespace Turing_Back_ED.Tests.DomainModels
{
    public class ProductStoreTest : IStoreTest<Product>
    {
        public readonly TestContext _context;
        public ProductStoreTest(TestContext context)
        {
            _context = context;
            _context.Products.AddAsync(new Product()
            {
                ProductId = 1,
                Name = "Product1",
                Price = 3.4m,
                Added = DateTime.UtcNow,
                Description = "Some cool product from Eddy"
            }).Wait();

            _context.Products.AddAsync(new Product()
            {
                ProductId = 2,
                Name = "Product2",
                Price = 17.2m,
                Added = DateTime.UtcNow,
                Description = "Second product from Eddy"
            }).Wait();
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products.ToListAsync();
        } 
            

        public void Add(Product entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> FindById(int Id)
        {
            return await _context.Products.FindAsync(Id);
        }

        public void Remove(Product entity)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Product Update(Product entity)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        #endregion
    }
}
