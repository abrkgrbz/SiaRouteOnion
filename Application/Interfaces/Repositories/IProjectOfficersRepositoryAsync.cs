using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.ProjectOfficers;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IProjectOfficersRepositoryAsync:IGenericRepositoryAsync<ProjectOfficers>
    {
        Task<AvailableUsersResponseDto> GetAvailableUsersForProjectAsync(int projectId, int pageNumber = 1, int pageSize = 3, string search = "");
        Task<ProjectUsersResponseDto> GetProjectUsersAsync(int projectId, int pageNumber = 1, int pageSize = 3, string search = "");
    }
}
