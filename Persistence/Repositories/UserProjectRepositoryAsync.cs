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
    public class UserProjectRepositoryAsync : GenericRepositoryAsync<UserProject>, IUserProjectRepositoryAsync
    {
        private readonly DbSet<UserProject> _userProjects;
        public UserProjectRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        {
            _userProjects = dbContext.Set<UserProject>();
        }
    }
}
