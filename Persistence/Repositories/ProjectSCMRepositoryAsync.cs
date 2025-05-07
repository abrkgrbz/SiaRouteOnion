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
    public class ProjectSCMRepositoryAsync : GenericRepositoryAsync<ProjectSCM>, IProjectSCMRepositoryAsync
    {
        private readonly DbSet<ProjectSCM> _projects;
        public ProjectSCMRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        {
            _projects = dbContext.Set<ProjectSCM>();
        }
    }
}
