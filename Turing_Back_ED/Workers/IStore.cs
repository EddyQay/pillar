using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;

namespace Turing_Back_ED.Workers
{
    public interface IStore<T> where T : IEntity 
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        void Remove(T entity);
        Task<IEnumerable<T>> GetAllAsync(GeneralQueryModel criteria);
        Task<T> FindByIdAsync(int Id);
        Task<IEnumerable<T>> FindAllAsync(SearchModel criteria);
        Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task<int> SaveChangesAsync();
    }
}
