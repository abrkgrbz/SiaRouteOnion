using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class ResponseRepositoryAsync:GenericRepositoryAsync<Response>, IResponseRepositoryAsync
    {
        private readonly DbSet<Response> _responses;
        public ResponseRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        {
            _responses = dbContext.Set<Response>();
        }
    }
}
