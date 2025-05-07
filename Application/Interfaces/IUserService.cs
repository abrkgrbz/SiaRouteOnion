using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.User;
using Identity.DTOs;

namespace Application.Interfaces
{
    public interface IUserService 
    {
        List<UserResponse> GetUserResponses(string name);
        Task<List<UserResponse>> GetAllUser();
        Task<IQueryable<UserResponse>> GetQueryableAllUser(); 
        Task<IQueryable<T>> OrderByField<T>(IQueryable<T> q, string sortField, bool ascending);
        Task<IQueryable<UserResponse>> GetQueryableAllUserByRoles();
        Task<List<UserResponseGroupedDepartmenDTO>> GetAllUserGroupByDepartman();
        Task<bool> ApproveUser(string userId);
        

    }
}
