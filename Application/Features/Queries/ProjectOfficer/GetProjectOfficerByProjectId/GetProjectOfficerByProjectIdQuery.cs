using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Queries.ProjectOfficer.GetProjectOfficerByProjectId
{
    public class GetProjectOfficerByProjectIdQuery:IRequest<Response<List<ProjectOfficersListViewModel>>>
    {
        public int ProjectId { get; set; }
    }

    public class GetProjectOfficerByProjectIdQueryHandler : IRequestHandler<GetProjectOfficerByProjectIdQuery,
        Response<List<ProjectOfficersListViewModel>>>
    {
        private readonly IMapper _mapper;
        private readonly IProjectOfficersRepositoryAsync _projectOfficersRepository;
        private readonly IUserService _userService;
        public GetProjectOfficerByProjectIdQueryHandler(IProjectOfficersRepositoryAsync projectOfficersRepository, IMapper mapper, IUserService userService)
        {
            _projectOfficersRepository = projectOfficersRepository;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Response<List<ProjectOfficersListViewModel>>> Handle(GetProjectOfficerByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var projectOfficers = await _projectOfficersRepository.GetWhereList(x => x.ProjectId == request.ProjectId);
            var users = await _userService.GetAllUser();
            var matchingUsers = (from officer in projectOfficers
                join user in users on officer.UserId equals user.Id
                select new ProjectOfficersListViewModel
                { 
                    FullName = user.FullName

                }).ToList();
            return new Response<List<ProjectOfficersListViewModel>>(matchingUsers );
        }
    }
}
