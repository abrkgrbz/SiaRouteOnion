using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.ProjectOfficers;
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
        private readonly IUserService _userService;
        public ProjectOfficersRepositoryAsync(SiaRouteDbContext dbContext, IUserService userService) : base(dbContext)
        {
            _userService = userService;
            _projectOfficers = dbContext.Set<ProjectOfficers>();
        }

        public async Task<AvailableUsersResponseDto> GetAvailableUsersForProjectAsync(int projectId, int pageNumber = 1, int pageSize = 3, string search = "")
        {
            var users = await _userService.GetAllUser();
            var userProjects = await _projectOfficers
                .Where(up => up.ProjectId == projectId)
                .Select(up => up.UserId)
                .ToListAsync();

            var availableUsers = users
                .Where(u => !userProjects.Contains(u.Id))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                availableUsers = availableUsers.Where(u =>
                    u.FullName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    (u.DepartmanName != null && u.DepartmanName.ToLower().Contains(search))
                );
            }

            int totalCount = availableUsers.Count();

            var pagedUsers = availableUsers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new AvailableUsersResponseDto
            {
                Users = pagedUsers,
                TotalCount = totalCount
            };

            return response;
        }

        public async Task<ProjectUsersResponseDto> GetProjectUsersAsync(int projectId, int pageNumber = 1, int pageSize = 3, string search = "")
        {
            var users = await _userService.GetAllUser();
            var userProjects = await _projectOfficers
                .Where(up => up.ProjectId == projectId)
                .Select(up => up.UserId)
                .ToListAsync();

            var availableUsers = users
                .Where(u => userProjects.Contains(u.Id))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                availableUsers = availableUsers.Where(u =>
                    u.FullName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    (u.DepartmanName != null && u.DepartmanName.ToLower().Contains(search))
                );
            }

            int totalCount = availableUsers.Count();

            var pagedUsers = availableUsers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new ProjectUsersResponseDto
            {
                Users = pagedUsers,
                TotalCount = totalCount
            };

            return response;
        }
    }
}

