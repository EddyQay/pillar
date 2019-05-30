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
using Attribute = Turing_Back_ED.Models.Attribute;

namespace Turing_Back_ED.DAL
{
    public class AttributeStore : IStore<Attribute>
    {
        private readonly DatabaseContext _context;
        private readonly TokenSection tokenSection;
        private readonly TokenManager tokenManager;

        public AttributeStore(DatabaseContext context, TokenManager _tokenManager, 
            IOptions<TokenSection> _tokenSection)
        {
            _context = context;
            tokenManager = _tokenManager;
            tokenSection = _tokenSection.Value;
        }

        public async Task<IEnumerable<Attribute>> GetAllAsync(GeneralQueryModel criteria)
        {
            IQueryable<Attribute> searchResult = null;
            criteria = criteria ?? new GeneralQueryModel();

            searchResult = _context.Attributes
                //for pagination eg. if page is given as 3, and
                //limit = 10, then pages to skip are 1 and 2
                //so skip = page -1 (that's equal to 2 pages)
                //then, the skip -> 2 x number of items per page
                //gives us number of items to skip, taking us
                //to where to start querying from.
                .Skip((criteria.Page - 1) * criteria.Limit)
                //once we know where to start, we query
                //the item count specified in the 'Limit' param
                .Take(criteria.Limit);
            return await searchResult.ToListAsync();
        }

        public async Task<IEnumerable<AttributeValue>> GetAttributeValues(int attribute_Id, GeneralQueryModel criteria)
        {
            return await _context.AttributeValues
                .Where(a => a.AttributeId == attribute_Id)
                .Skip(criteria.Limit * (criteria.Page - 1))
                .Take(criteria.Limit).ToListAsync();
        }

        public async Task<IEnumerable<object>> GetProductAttributes(int product_Id, GeneralQueryModel criteria)
        {
            return await _context.ProductAttributes
                .Include(a => a.AttributeValue)
                .Include(a => a.AttributeValue.Attribute)
                .Where(a => a.ProductId == product_Id)
                .Skip(criteria.Limit * (criteria.Page - 1))
                .Take(criteria.Limit)
                .Select(a => new {
                    AttributeName = a.AttributeValue.Attribute.Name,
                    a.AttributeValueId,
                    AttributeValue = a.AttributeValue.Value
                }).ToListAsync();

        }

        public async Task<Attribute> FindByIdAsync(int Id)
        {
            return await _context.Attributes.FindAsync(Id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #region  NOT IMPLEMENTED

        public Task<Attribute> AddAsync(Attribute entity)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseModel> AddAsync(Attribute attribute, int i)
        {
            throw new NotImplementedException();
        }

        public Task<Attribute> UpdateAsync(Attribute entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Attribute entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Attribute>> FindAllAsync(SearchModel criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Attribute>> FindByConditionAsync(Expression<Func<Attribute, bool>> expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
