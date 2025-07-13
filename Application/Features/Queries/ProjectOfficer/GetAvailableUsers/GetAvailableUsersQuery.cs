using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.User;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Queries.ProjectOfficer.GetAvailableUsers
{
    public class GetAvailableUsersQuery:IRequest<List<GetAvailableUsersResponse>>
    {
        public int ProjectId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public string SearchTerm { get; set; } = "";
    }

    public class GetAvailableUsersResponse
    {
        public List<UserResponse> UserResponses { get; set; }
        public int TotalCount { get; set; }
    }

    public class GetAvailableUsersQueryHandler : IRequestHandler<GetAvailableUsersQuery, List<GetAvailableUsersResponse>>
    {
        private readonly IProjectOfficersRepositoryAsync _projectOfficersRepositoryAsync;

        public GetAvailableUsersQueryHandler(IProjectOfficersRepositoryAsync projectOfficersRepositoryAsync)
        {
            _projectOfficersRepositoryAsync = projectOfficersRepositoryAsync;
        }

        public async Task<List<GetAvailableUsersResponse>> Handle(GetAvailableUsersQuery request, CancellationToken cancellationToken)
        {
            var data =await _projectOfficersRepositoryAsync.GetAvailableUsersForProjectAsync(request.ProjectId,
                request.PageNumber, request.PageSize, request.SearchTerm);
            return new List<GetAvailableUsersResponse>
            {
                new GetAvailableUsersResponse
                {
                    UserResponses = data.Users,
                    TotalCount = data.TotalCount
                }
            };
        }
    }
}
