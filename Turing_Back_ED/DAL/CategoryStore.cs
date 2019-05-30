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
    public class CategoryStore : IStore<Category>
    {
        private readonly DatabaseContext _context;
        private readonly TokenSection tokenSection;
        private readonly TokenManager tokenManager;

        public CategoryStore(DatabaseContext context, TokenManager _tokenManager, 
            IOptions<TokenSection> _tokenSection)
        {
            _context = context;
            tokenManager = _tokenManager;
            tokenSection = _tokenSection.Value;
        }

        public Task<IEnumerable<Category>> GetAllAsync(GeneralQueryModel criteria)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CategoryQueryModel criteria)
        {
            IQueryable<Category> searchResult = null;
            criteria = criteria ?? new CategoryQueryModel();

            if (string.IsNullOrWhiteSpace(criteria.Order))
            {
                searchResult = _context.Categories
                    .OrderBy(a => a.CategoryId)
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
            }

            var FieldAndSortOrder = criteria.Order.Trim().Split(",");
            var sortValues = Constants.AllowedSorts[FieldAndSortOrder[0]].Split("|").ToList();

            switch (FieldAndSortOrder[0])
            {
                case Constants.ValidSortFileds_category_id:
                    searchResult = FieldAndSortOrder[1].Equals(Constants.SortOrderNames.DESC.ToString())
                        ? _context.Categories.OrderByDescending(a => a.CategoryId)
                                .Skip((int)((criteria.Page - 1) * criteria.Limit))
                                .Take((int)criteria.Limit)
                        : _context.Categories.OrderBy(a => a.CategoryId)
                                .Skip((int)((criteria.Page - 1) * criteria.Limit))
                                .Take((int)criteria.Limit);
                    break;
                case Constants.ValidSortFileds_name:
                    searchResult = FieldAndSortOrder[1].Equals(Constants.SortOrderNames.DESC.ToString())
                        ? _context.Categories.OrderByDescending(a => a.Name)
                                .Skip((int)((criteria.Page - 1) * criteria.Limit))
                                .Take((int)criteria.Limit)
                        : _context.Categories.OrderBy(a => a.Name)
                                .Skip((int)((criteria.Page - 1) * criteria.Limit))
                                .Take((int)criteria.Limit);
                    break;
                default:
                    searchResult = FieldAndSortOrder[1].Equals(Constants.SortOrderNames.DESC.ToString())
                        ? _context.Categories.OrderBy(a => a.CategoryId)
                                .Skip((int)((criteria.Page - 1) * criteria.Limit))
                                .Take((int)criteria.Limit)
                        : _context.Categories.OrderBy(a => a.CategoryId)
                                .Skip((int)((criteria.Page - 1) * criteria.Limit))
                                .Take((int)criteria.Limit);
                    break;
            }

            return await searchResult.ToListAsync();
        }

        public (bool valid, Constants.ErrorCodes errorType, 
            string message) IsSortOrderValid(string sortOrder)
        {
            var keyAndValue = sortOrder.Trim().Split(",");
            if (Constants.AllowedSorts.Keys.Contains(keyAndValue[0]))
            {
                var sortValues = Constants.AllowedSorts[keyAndValue[0]].Split("|").ToList();
                if (sortValues.Contains(keyAndValue[1]))
                {
                    return (true, Constants.ErrorCodes.PAG_01, string.Empty);
                }

                return (false, Constants.ErrorCodes.PAG_01, Constants.ErrorMessages.PAG_01);
            }

            return (false, Constants.ErrorCodes.PAG_02, Constants.ErrorMessages.PAG_02); ;
        }

        public async Task<IEnumerable<AttributeValue>> GetAttributeValues(int attribute_Id, GeneralQueryModel criteria)
        {
            return await _context.AttributeValues
                .Where(a => a.AttributeId == attribute_Id)
                .Skip(criteria.Limit * (criteria.Page - 1))
                .Take(criteria.Limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetProductCategories(int product_Id, GeneralQueryModel criteria)
        {
            return await _context.ProductCategories
                .Include(a => a.Category)
                .Where(a => a.ProductId == product_Id)
                .Skip(criteria.Limit * (criteria.Page - 1))
                .Take(criteria.Limit)
                .Select(a => new {
                    a.CategoryId,
                    a.Category.DepartmentId,
                    a.Category.Name
                })
                .ToListAsync();

        }

        public async Task<IEnumerable<object>> GetDepartmentCategories(int department_Id, GeneralQueryModel criteria)
        {
            return await _context.Categories
                .Include(a => a.Department)
                .Where(a => a.DepartmentId == department_Id)
                .Skip(criteria.Limit * (criteria.Page - 1))
                .Take(criteria.Limit)
                .ToListAsync();

        }

        public async Task<Category> FindByIdAsync(int Id)
        {
            return await _context.Categories.FindAsync(Id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #region NOT IMPLEMENTED

        public Task<Category> AddAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task<Category> UpdateAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Category entity)
        {
            throw new NotImplementedException();
        }

        Task<Category> IStore<Category>.FindByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> FindAllAsync(SearchModel criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> FindByConditionAsync(Expression<Func<Category, bool>> expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
