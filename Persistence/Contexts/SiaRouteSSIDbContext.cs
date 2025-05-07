using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.SSI;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts
{
    public class SiaRouteSSIDbContext: DbContext
    {
        public SiaRouteSSIDbContext(DbContextOptions<SiaRouteSSIDbContext> options)
            : base(options)
        {
        }
        public DbSet<Map> Maps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Map>().HasNoKey();
        }
        public async Task<List<Map>> GetDynamicMapList(string tableName)
        {
           var sqlQuery = $"SELECT * FROM {tableName}";
          return await Set<Map>().FromSqlRaw(sqlQuery).ToListAsync();

        }

    }
}
