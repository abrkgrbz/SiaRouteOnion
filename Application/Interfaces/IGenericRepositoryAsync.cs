using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetAsync(Expression<Func<T,bool>> method,bool tracking=false);
        Task<IReadOnlyList<T>> GetAllAsync(); 
        Task<IReadOnlyList<T>> GetWhereList(Expression<Func<T,bool>> method,bool tracking = false);
        Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize);
        Task<T> AddAsync(T entity,CancellationToken token);
        
        Task<bool> UpdateAsync(T entity,CancellationToken token);
        bool Update(T entity );
        Task<bool> DeleteAsync(T entity, CancellationToken token);
        Task<bool> AddRangeAsync(List<T> datas,CancellationToken token);
        Task<IQueryable<T>> OrderByField<T>(IQueryable<T> q, string sortField, bool ascending);
        IQueryable<T> GetAll(bool tracking = true);
        IQueryable<T> Include(params Expression<Func<T, object>>[] includes);  

    }
}
