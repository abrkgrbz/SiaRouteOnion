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
    public class ProjectMethodsRepositoryAsync:GenericRepositoryAsync<ProjectMethods>,IProjectMethodsRepositoryAsync
    {
        private readonly DbSet<ProjectMethods> _projectMethods;
        public ProjectMethodsRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        {
            _projectMethods = dbContext.Set<ProjectMethods>();
        }

        
    }
}
