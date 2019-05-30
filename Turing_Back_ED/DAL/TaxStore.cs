using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Turing_Back_ED.DAL;
using Turing_Back_ED.Models;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.DomainModels
{
    public class TaxStore : IStore<Tax>
    {
        private readonly IUserManager userManager;
        private readonly IAuthenticationManager authManager;
        private readonly DatabaseContext _context;
        private readonly TokenSection tokenSection;
        private readonly TokenManager tokenManager;

        public TaxStore(DatabaseContext context, TokenManager _tokenManager, 
            IAuthenticationManager _authManager, IUserManager _userManager, 
            IOptions<TokenSection> _tokenSection)
        {
            authManager = _authManager;
            userManager = _userManager;
            _context = context;
            tokenManager = _tokenManager;
            tokenSection = _tokenSection.Value;
        }

        public async Task<IEnumerable<Tax>> GetAllAsync(GeneralQueryModel criteria)
        {
            IQueryable<Tax> searchResult = null;
            criteria = criteria ?? new GeneralQueryModel();

            searchResult = _context.Taxes
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
        
        public async Task<Tax> FindByIdAsync(int Id)
        {
            return await _context.Taxes.FindAsync(Id);
        }

        //NOT IMPLEMENTED
        public Task<Tax> AddAsync(Tax entity)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseModel> AddAsync(Tax department, int i)
        {
            throw new NotImplementedException();
        }

        public Task<Tax> UpdateAsync(Tax entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Tax entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tax>> FindAllAsync(SearchModel criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tax>> FindByConditionAsync(Expression<Func<Tax, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
