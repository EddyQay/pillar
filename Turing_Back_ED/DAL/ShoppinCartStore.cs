using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.DAL
{
    public class ShopppingCartStore : IStore<ShoppingCart>
    {
        private readonly IUserManager userManager;
        private readonly IAuthenticationManager authManager;
        private readonly TuringshopContext _context;
        private readonly TokenSection tokenSection;
        private readonly TokenManager tokenManager;

        public ShopppingCartStore(TuringshopContext context, TokenManager _tokenManager, 
            IAuthenticationManager _authManager, IUserManager _userManager, 
            IOptions<TokenSection> _tokenSection)
        {
            authManager = _authManager;
            userManager = _userManager;
            _context = context;
            tokenManager = _tokenManager;
            tokenSection = _tokenSection.Value;
        }

        public async Task<IEnumerable<Shipping>> GetAllAsync(GeneralQueryModel criteria)
        {
            IQueryable<Shipping> searchResult = null;
            criteria = criteria ?? new GeneralQueryModel();

            searchResult = _context.Shippings
                //for pagination eg. if page is given as 3, and
                //limit = 10, then pages to skip are 1 and 2
                //so skip = page -1 (that's equal to 2 pages)
                //then, the skip -> 2 x number of items per page
                //gives us number of items to skip, taking us
                //to where to start querying from.
                .Skip((int)((criteria.Page - 1) * criteria.Limit))
                //once we know where to start, we query
                //the item count specified in the 'Limit' param
                .Take((int)criteria.Limit);
            return await searchResult.ToListAsync();
        }

        public async Task<IEnumerable<ShippingRegion>> GetAllShippingRegions(GeneralQueryModel criteria)
        {
            return await _context.ShippingRegions
                .Skip(criteria.Limit * (criteria.Page - 1))
                .Take(criteria.Limit).ToListAsync();
        }
        
        public async Task<Shipping> FindByIdAsync(int Id)
        {
            return await _context.Shippings.FindAsync(Id);
        }

        public async Task<ShippingRegion> FindShippingRegionById(int Id)
        {
            return await _context.ShippingRegions.FindAsync(Id);
        }

        #region NOT IMPLEMENTED
        public Task<ShoppingCart> AddAsync(ShoppingCart entity)
        {
            throw new NotImplementedException();
        }

        public Task<ShoppingCart> UpdateAsync(ShoppingCart entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(ShoppingCart entity)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ShoppingCart>> IStore<ShoppingCart>.GetAllAsync(GeneralQueryModel criteria)
        {
            throw new NotImplementedException();
        }

        Task<ShoppingCart> IStore<ShoppingCart>.FindByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ShoppingCart>> FindAllAsync(SearchModel criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ShoppingCart>> FindByConditionAsync(Expression<Func<ShoppingCart, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
