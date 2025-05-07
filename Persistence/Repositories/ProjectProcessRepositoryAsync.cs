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
    public class ProjectProcessRepositoryAsync:GenericRepositoryAsync<ProjectProcess>,IProjectProcessRepositoryAsync
    {
        private readonly DbSet<ProjectProcess> _projectProcesses;
        public ProjectProcessRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        {
            _projectProcesses = dbContext.Set<ProjectProcess>();
        }
    }
}
