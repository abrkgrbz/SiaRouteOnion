using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.User;
using Application.Enums;
using Application.Interfaces;
using Identity.DTOs;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<SiaRouteUser> _userManager;

        public UserService(UserManager<SiaRouteUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserResponse> GetUserResponses(string name)
        {
            var data = _userManager.Users.Where(x => x.UserName.Contains(name)).ToList();
            return data.Select(x => new UserResponse
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                FullName = x.FirstName + " " + x.LastName,
            }).ToList();
        }

        public async Task<List<UserResponse>> GetAllUser()
        {
            var data = await _userManager.Users.AsNoTracking().ToListAsync();
            return data.Select(x => new UserResponse
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                FullName = x.FirstName + " " + x.LastName,
                Name = x.FirstName,
                LastName = x.LastName
            }).ToList();
        }

        public async Task<IQueryable<UserResponse>> GetQueryableAllUser()
        {
            var users = await _userManager.Users.AsNoTracking()
                .Include(x => x.Department)
                .ToListAsync();
            List<UserResponse> userResponses = new List<UserResponse>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userResponses.Add(new UserResponse()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    EmailConfirmed = user.EmailConfirmed,
                    FullName = user.FirstName + " " + user.LastName,
                    Roles = string.Join(",", roles),
                    DepartmanName = user.Department.DepartmentName
                });
            }
            return userResponses.AsQueryable();
        }

        public async Task<IQueryable<T>> OrderByField<T>(IQueryable<T> q, string sortField, bool ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, sortField);
            var exp = Expression.Lambda(prop, param);
            string method = ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return (IQueryable<T>)q.Provider.CreateQuery<T>(mce);
        }

        public async Task<IQueryable<UserResponse>> GetQueryableAllUserByRoles()
        {
            var users = await _userManager.Users.AsNoTracking().Include(x => x.Department).Include(x => x.Department).ToListAsync();
            List<UserResponse> userResponses = new List<UserResponse>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userResponses.Add(new UserResponse()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    EmailConfirmed = user.EmailConfirmed,
                    FullName = user.FirstName + " " + user.LastName,
                    Roles = string.Join(",", roles),
                    DepartmanName = user.Department.DepartmentName
                });
            }
            return userResponses.AsQueryable();
        }

        public async Task<List<UserResponseGroupedDepartmenDTO>> GetAllUserGroupByDepartman()
        {
            var users = await _userManager.Users.AsNoTracking()
                .Include(x => x.Department)
                .ToListAsync();
            var groupedUsers = users.GroupBy(x => x.Department.DepartmentName);
            List<UserResponseGroupedDepartmenDTO> userResponses = new();
            foreach (var group in groupedUsers)
            {
                userResponses.Add(new UserResponseGroupedDepartmenDTO()
                {
                    DepartmanName = group.Key,
                    Users = group.Select(u => new UserDto()
                    {
                        FullName = u.FirstName + " " + u.LastName,
                        UserId = u.Id
                    }).ToList()
                });
            }

            return userResponses;
        }

        public async Task<bool> ApproveUser(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id.Contains(userId));
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
            }

            var updateUser = await _userManager.UpdateAsync(user);
            if (updateUser.Succeeded)
            {
                return true;
            }

            return false;
        }
    }
}
