using Application.Interfaces.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class SCMRepositoryAsync:GenericRepositoryAsync<SCM>,ISCMRepositoryAsync
    {
        private readonly DbSet<SCM> _scm;
        public SCMRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        {
            _scm = dbContext.Set<SCM>();
        }
    }
}
