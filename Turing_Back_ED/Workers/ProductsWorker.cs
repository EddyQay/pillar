using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Turing_Back_ED.Workers;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.DomainModels
{
    public class ProductsWorker : IStore<Product>, IDisposable
    {
        private readonly DatabaseContext _context;
        private readonly TokenManager tokenManager;

        public ProductsWorker(DatabaseContext context, TokenManager _tokenGetter)
        {
            _context = context;
            tokenManager = _tokenGetter;
        }
        
        
        public async Task<IEnumerable<Product>> GetAllAsync(GeneralQueryModel criteria = null)
        {
            IQueryable<Product> searchResult = null;
            criteria = criteria ?? new GeneralQueryModel();

            searchResult = _context.Products
                //for pagination eg. if page is given as 3, and
                //limit = 10, then pages to skip are 1 and 2
                //so skip = page -1 (that's equal to 2 pages)
                //then, the skip -> 2 x number of items per page
                //gives us number of items to skip, taking us
                //to where to start querying from.
                .Skip( (int)((criteria.Page - 1) * criteria.Limit))
                //once we know where to start, we query
                //the item count specified in the 'Limit' param
                .Take((int)criteria.Limit).AsNoTracking();

            if (searchResult != null)
                foreach (Product p in searchResult)
                {
                    if (p.Description.Length > criteria.Description_Length)
                        p.Description = p.Description.Substring(0, (int)criteria.Description_Length - 3) + "...";
                }

            return await searchResult.ToListAsync();
        }

        public async Task<Product> AddAsync(Product entity)
        {
            _context.Products.Add(entity);
            _context.Entry(entity).State = EntityState.Added;
            int result =  await SaveChangesAsync();
            return result > 0 ? entity : null;
        }

        public async Task<Product> FindByIdAsync(int Id)
        {
            return await _context.Products.FindAsync(Id);
        }

        public async Task<int> FindByIdCountAsync(int Id)
        {
            return await _context.Products.Where(p => p.ProductId == Id).CountAsync();
        }

        public async Task<ProductDetail> FindById_D(int Id)
        {
            var result = await _context.Products.FindAsync(Id);

            if (result != null)
                return new ProductDetail
                {
                    ProductId = result.ProductId,
                    Name = result.Name,
                    Description = result.Description,
                    Price = result.Price,
                    DiscountedPrice = result.DiscountedPrice,
                    Image = result.Image,
                    Image2 = result.Image2
                };
            else
                return null;
        }

        public async Task<IEnumerable<Product>> FindAllAsync(SearchModel criteria)
        {
            IEnumerable<Product> searchResult = null;
            switch (criteria.All_Words)
            {
                case "on":
                    searchResult = await _context.Products
                        //find all products names that contain 'Query_String' value
                        .Where(p => p.Name.Contains(criteria.Query_String) ||
                        //or product description contains 'Query_String' value
                        p.Description.Contains(criteria.Query_String))
                        .Skip(criteria.Limit * (criteria.Page - 1))
                        .Take(criteria.Limit).ToListAsync();
                    break;
                case "off":
                    searchResult = await _context.Products
                        //find all products whose names that DO NOT 
                        //contain 'Query_String' value
                        .Where(p => !p.Name.Contains(criteria.Query_String) &&
                        //AND descriptions DO NOT contain 'Query_String' value
                        !p.Description.Contains(criteria.Query_String))
                        .Skip(criteria.Limit * (criteria.Page - 1))
                        .Take(criteria.Limit).ToListAsync();
                    break;
                default:
                    searchResult = await _context.Products
                        .Where(p => p.Name.Contains(criteria.Query_String) ||
                        p.Description.Contains(criteria.Query_String))
                        .Skip(criteria.Limit * (criteria.Page - 1))
                        .Take(criteria.Limit).ToListAsync();
                    break;
            }

            if(searchResult != null)
            foreach (Product p in searchResult)
            {
                    if (p.Description.Length > criteria.Description_Length)
                        p.Description = p.Description.Substring(0, criteria.Description_Length-3)+"...";
            }

            return searchResult;
        }

        public async Task<IEnumerable<Product>> FindInCategory(int categoryId, GeneralQueryModel criteria)
        {
            return await _context.Products
                .Where(a => a.CategoryId == categoryId)
                .Skip(criteria.Limit * (criteria.Page - 1))
                .Take(criteria.Limit).ToListAsync();
        }

        public async Task<IEnumerable<Product>> FindInDepartment(int departmentId, GeneralQueryModel criteria)
        {
            return await _context.Products
                .Where(a => a.Category.DepartmentId == departmentId)
                .Skip(criteria.Limit * (criteria.Page - 1))
                .Take(criteria.Limit)
                .ToListAsync();
        }

        public async Task<ProductLocation> FindLocations(int productId)
        {
            return await _context.Products
                .Include(a => a.Category)
                .Include(a => a.Category.Department)
                .Where(a => a.ProductId == productId)
                .Select(a => new ProductLocation
                {
                    CategoryId = (int)a.CategoryId,
                    CategoryName = a.Category.Name,
                    DepartmentId = a.Category.DepartmentId,
                    DepartmentName = a.Category.Department.Name
                }).FirstOrDefaultAsync();
        }


        public int AddReview(Review_ model, HttpContext httpContext)
        {
            var id = _context.Reviews.Any() 
                ? _context.Reviews.Max(p => p.ReviewId) + 1
                : 1;

            int custId = Convert.ToInt32(httpContext.User.Identity.Name);

            if (custId > 0)
            {
                model.CreatedOn = DateTime.UtcNow;
                model.CustomerId = custId;
                var entry = _context.Reviews.Add(model);

                entry.State = EntityState.Added;
                return id;
            }
            else
            {
                return -1;
            }
        }

        public async Task<IEnumerable<Review_>> GetReviews(int productId)
        {
            return await _context.Reviews
                .Where(a => a.ProductId == productId)
                .ToListAsync();
        }

        public void Remove(Product entity)
        {
            //remove entity from context
            _context.Products.Remove(entity);
        }

        public async Task<Product> UpdateAsync(Product entity)
        {
            //mark entity as modified, so to be
            //modified in the database when
            //'SaveChanges' is called
            _context.Entry(entity).State = EntityState.Modified;

            await SaveChangesAsync();
            return entity;
        }


        public async Task<int> SaveChangesAsync()
        {
            //save changes to database using non-blocking
            //mechanism
            return await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<Product>> FindByConditionAsync(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //dispose managed state (managed objects).
                    _context.Dispose();
                }

                //free unmanaged resources (unmanaged objects) and override a finalizer below.

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ProductStore() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
