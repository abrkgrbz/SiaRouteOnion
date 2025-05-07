using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Persistence.Contexts;
using Persistence.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace Persistence.Repositories
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly SiaRouteDbContext _dbContext;
        public GenericRepositoryAsync(SiaRouteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DbSet<T> Table => _dbContext.Set<T>();
        public virtual async Task<T> GetByIdAsync(int id)
        {

            return await Table.FindAsync(id);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> method, bool tracking = false)
        {
            return await (tracking ? Table.FirstOrDefaultAsync(method) : Table.AsNoTracking().FirstOrDefaultAsync(method));
        }

        public async Task<IReadOnlyList<T>> GetWhereList(Expression<Func<T, bool>> method, bool tracking = false)
        {
            var query = Table.Where(method);
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize)
        {
            return await Table
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T> AddAsync(T entity, CancellationToken token)
        {
            await Table.AddAsync(entity, token);
            await _dbContext.SaveChangesAsync(token);
            return entity;
        }



        public async Task<bool> UpdateAsync(T entity, CancellationToken token)
        {
            try
            {
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                {
                    _dbContext.Attach(entity);
                }
                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync(token);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Update(T entity)
        {
            Table.Update(entity);
            int result = _dbContext.SaveChanges();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(T entity, CancellationToken token)
        {
            try
            {
                Table.Remove(entity);
                await _dbContext.SaveChangesAsync(token);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> AddRangeAsync(List<T> datas, CancellationToken token)
        {
            await Table.AddRangeAsync(datas);
            await _dbContext.SaveChangesAsync(token);
            return true;
        }

        public async Task<IQueryable<T>> OrderByField<T>(IQueryable<T> q, string sortField, bool ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, sortField);
            var exp = Expression.Lambda(prop, param);
            string method = ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return (IQueryable<T>)q.Provider.CreateQuery<T>(mce);
        }

        public IQueryable<T> GetAll(bool tracking = false)
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = query.AsNoTracking();
            return query;
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = Table;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await Table
                .ToListAsync();
        }


    }
}
