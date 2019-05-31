using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Turing_Back_ED.Tests.DAL
{
    public interface IStoreTest<T> where T : Turing_Back_ED.Workers.IEntity
    {
        void Add(T entity);
        T Update(T entity);
        void Remove(T entity);
        Task<IEnumerable<T>> GetAll();
        Task<T> FindById(int Id);
        void SaveChanges();
    }
}
