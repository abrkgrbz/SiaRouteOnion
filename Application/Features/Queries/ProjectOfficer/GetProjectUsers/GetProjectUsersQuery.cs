using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.ProjectOfficers;
using Application.DTOs.User;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Queries.ProjectOfficer.GetProjectUsers
{
    public  class GetProjectUsersQuery:IRequest<ProjectUsersResponse>
    {
        public int ProjectId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = ""; 
    }

    public class ProjectUsersResponse
    {
        public ProjectUsersResponseDto ProjectUsersResponseDto { get; set; } = new ProjectUsersResponseDto();
    }

    public class GetProjectUsersQueryHandler : IRequestHandler<GetProjectUsersQuery, ProjectUsersResponse>
    {
        private readonly IProjectOfficersRepositoryAsync _projectOfficersRepositoryAsync;

        public GetProjectUsersQueryHandler(IProjectOfficersRepositoryAsync projectOfficersRepositoryAsync)
        {
            _projectOfficersRepositoryAsync = projectOfficersRepositoryAsync;
        }

        public async Task<ProjectUsersResponse> Handle(GetProjectUsersQuery request, CancellationToken cancellationToken)
        {
            var data = await _projectOfficersRepositoryAsync.GetProjectUsersAsync(request.ProjectId, request.PageNumber,
                request.PageSize, request.SearchTerm);

            return new ProjectUsersResponse() { ProjectUsersResponseDto = data };
        }
    }
}
