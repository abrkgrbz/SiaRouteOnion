using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class ProjectOfficersRepositoryAsync:GenericRepositoryAsync<ProjectOfficers>,IProjectOfficersRepositoryAsync
    {
        private readonly DbSet<ProjectOfficers> _projectOfficers;
        public ProjectOfficersRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        {
            _projectOfficers = dbContext.Set<ProjectOfficers>();
        }
    }
}

