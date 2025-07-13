using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class UserProjectRepositoryAsync : GenericRepositoryAsync<UserProject>, IUserProjectRepositoryAsync
    {
        private readonly DbSet<UserProject> _userProjects;
        private readonly IUserService _userService;
        public UserProjectRepositoryAsync(SiaRouteDbContext dbContext, IUserService userService) : base(dbContext)
        {
            _userService = userService;
            _userProjects = dbContext.Set<UserProject>();
        }

    }
}
