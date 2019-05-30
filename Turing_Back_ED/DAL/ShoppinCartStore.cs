using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public class ShoppingCartStore : IStore<ShoppingCart>
    {
        private readonly DatabaseContext _context;
        private readonly TokenSection tokenSection;
        private readonly TokenManager tokenManager;

        public ShoppingCartStore(DatabaseContext context, TokenManager _tokenManager,
            IOptions<TokenSection> _tokenSection)
        {
            _context = context;
            tokenManager = _tokenManager;
            tokenSection = _tokenSection.Value;
        }

        public string GenerateUniqueId()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<ShoppingCart> AddAsync(ShoppingCart entity)
        {
            entity.BuyNow = true;
            entity.Added = DateTime.UtcNow;
            entity.Quantity = 1;
            await _context.ShoppingCarts.AddAsync(entity);
            await SaveChangesAsync();
            return await FindByIdVerboseAsync(entity.ItemId);
        }

        public async Task<IEnumerable<ShoppingCartProductItem>> FindAllByCartIdAsync(Guid cartId, GeneralQueryModel criteria)
        {
            var itemsList = await _context.ShoppingCarts
                .Where(c => c.CartId.Equals(cartId))
                .OrderBy(c => c.ItemId)
                .Include(item => item.Product)
                .Skip((int)((criteria.Page - 1) * criteria.Limit))
                //once we know where to start, we query
                //the item count specified in the 'Limit' param
                .Take((int)criteria.Limit)
                .ToListAsync();

            return ShoppingCartProductItems.From(itemsList);
        }

        public async Task<IEnumerable<ShoppingCartProductItem>> FindAllByCartIdAsync(Guid cartId)
        {
            var itemsList = await _context.ShoppingCarts
                .Where(c => c.CartId.Equals(cartId))
                .OrderBy(c => c.ItemId)
                .Include(item => item.Product)
                .ToListAsync();

            return ShoppingCartProductItems.From(itemsList);
        }

        public IEnumerable<ShoppingCartProductItem> FindAllActiveByCartIdAsync(Guid cartId)
        {
            var itemsList = _context.ShoppingCarts
                .Where(c => c.CartId.Equals(cartId) && c.BuyNow == true)
                .OrderBy(c => c.ItemId)
                .Include(item => item.Product)
                .AsEnumerable();

            return ShoppingCartProductItems.From(itemsList);
        }
        

        public async Task<ShoppingCart> UpdateAsync(int itemId, int quantity)
        {
            var item = await FindByIdAsync(itemId);
            item.Quantity = (quantity > 0) ? quantity : item.Quantity;

            return await UpdateAsync(item);
        }

        public async Task<ShoppingCart> UpdateAsync(ShoppingCart entity)
        {
            entity.Modified = DateTime.UtcNow;
            _context.ShoppingCarts.Update(entity);
            await SaveChangesAsync();
            return await FindByIdVerboseAsync(entity.ItemId);
        }

        public async Task<IEnumerable<ShoppingCart>> EmptyCart(Guid cartId)
        {
            var cartItems = _context.ShoppingCarts
                .Where(cart => cart.CartId.Equals(cartId));

            _context.ShoppingCarts.RemoveRange(cartItems);

            await SaveChangesAsync();
            return new List<ShoppingCart>();
        }

        public async Task<int> MoveItemToCart(int itemId)
        {
            var cartItem = await FindByIdAsync(itemId);
            if (cartItem != null)
            {
                cartItem.BuyNow = true;
                await UpdateAsync(cartItem);
                return 1;
            }

            return -1;
        }

        public async Task<int> SaveItemForLater(int itemId)
        {
            var cartItem = await FindByIdAsync(itemId);
            if (cartItem != null)
            {
                cartItem.BuyNow = false;
                await UpdateAsync(cartItem);
                return 1;
            }

            return -1;
        }

        public decimal GetTotalItemCountForCart(Guid cartId)
        {
            //If we need the total amount for shopping cart 
            //in terms of money

            var cartProductItems = FindAllActiveByCartIdAsync(cartId);

            return ShoppingCartProductItems
                .ComputeTotalAmount(cartProductItems);

            //else, if the goal of this is to get the total items
            //in the shopping cart, the we'll uncomment this block

            /*return await _context.ShoppingCarts
                .Where(cart => cart.CartId.Equals(cartId))
                .CountAsync();*/


        }

        public async Task<IEnumerable<ShoppingCartProductItem>> GetSavedItemsInCart(Guid cartId)
        {
            var cartItems = await _context.ShoppingCarts
                .Where(cart => 
                       cart.CartId.Equals(cartId) && 
                       cart.BuyNow == false)
                .Include(item => item.Product)
                .ToListAsync();

            return ShoppingCartProductItems.From(cartItems);
        }

        public async Task<int> Remove(ShoppingCart entity)
        {
            _context.ShoppingCarts.Remove(entity);
            return await SaveChangesAsync();
        }

        public async Task<int> Remove(int entity)
        {
            var itemToRemove = await FindByIdAsync(entity);
            if (itemToRemove != null)
            {
                _context.ShoppingCarts.Remove(itemToRemove);
                return await SaveChangesAsync();
            }

            return -1;
        }

        public async Task<ShoppingCart> FindByIdAsync(int Id)
        {
            return await _context.ShoppingCarts.FindAsync(Id);
        }

        public async Task<ShoppingCart> FindByIdVerboseAsync(int Id)
        {
            return await _context.ShoppingCarts
                .Include(cart => cart.Product)
                //.Include(cart => cart.Product.Category)
                .Where(item => item.ItemId == Id)
                .SingleOrDefaultAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllAsync(GeneralQueryModel criteria)
        {
            IQueryable<ShoppingCart> searchResult = null;
            criteria = criteria ?? new GeneralQueryModel();

            searchResult = _context.ShoppingCarts
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
        

        #region NOT IMPLEMENTED        

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

        void IStore<ShoppingCart>.Remove(ShoppingCart entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
