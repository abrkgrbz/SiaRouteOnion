using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ComplexModel;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Queries.Project.GetAllProject
{
    public class GetAllProjectQuery : IRequest<DataTableReponse<IReadOnlyList<GetAllProjectVM>>>
    {
        public int draw { get; set; }
        public int Start{ get; set; }
        public int Length { get; set; }
        public string? orderColumnIndex { get; set; }
        public string? orderDir { get; set; }
        public string? orderColumnName { get; set; }
        public string? searchValue { get; set; }
    }

    public class GetAllProjectQueryHandler : IRequestHandler<GetAllProjectQuery, DataTableReponse<IReadOnlyList<GetAllProjectVM>>>
    {
        private readonly IAuthenticatedUserService _authenticatedUserService;
        private readonly IProjectOfficersRepositoryAsync _projectOfficersRepositoryAsync;
        private readonly IUserProjectRepositoryAsync _userProjectRepositoryAsync;
        private IProjectRepositoryAsync _projectRepository;
        private IMapper _mapper;
        public GetAllProjectQueryHandler(IProjectRepositoryAsync projectRepository, IMapper mapper, IAuthenticatedUserService authenticatedUserService, IUserProjectRepositoryAsync userProjectRepositoryAsync, IProjectOfficersRepositoryAsync projectOfficersRepositoryAsync)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _authenticatedUserService = authenticatedUserService;
            _userProjectRepositoryAsync = userProjectRepositoryAsync;
            _projectOfficersRepositoryAsync = projectOfficersRepositoryAsync;
        }
        public async Task<DataTableReponse<IReadOnlyList<GetAllProjectVM>>> Handle(GetAllProjectQuery request, CancellationToken cancellationToken)
        {


            string userId = _authenticatedUserService.UserId;
            List<string> roles = _authenticatedUserService.UserRoles;
            int recordsFiltered = 0, recordTotal = 0;
            IQueryable<ProjectList> projects;

            if (roles.Contains("Admin") || roles.Contains("SuperAdmin"))
            {
                projects = await _projectRepository.GetAllProjectsComplex();
            } 
            else
            {
                var projectOfficers = await _projectOfficersRepositoryAsync.GetWhereList(x => x.UserId == userId);
                var projectOfficerIds = projectOfficers.Select(x => x.ProjectId).ToList();

                var userProjects = await _userProjectRepositoryAsync.GetWhereList(x => x.UserId == userId);
                var userProjectIds = userProjects.Select(x => x.ProjectId).ToList();

                var projectIds = projectOfficerIds.Union(userProjectIds).Distinct().ToList(); 

                projects =  _projectRepository.GetAllProjectsComplex().Result.Where(x => projectIds.Contains(x.ProjectId));

            }
            
            if (!projects.Any())
            {
                return new DataTableReponse<IReadOnlyList<GetAllProjectVM>>(new List<GetAllProjectVM>(), recordsFiltered, recordTotal);
            } 

            if (!string.IsNullOrEmpty(request.searchValue))
            {
                projects = projects.Where(x => x.ProjectName.Contains(request.searchValue.ToLower()));
            }
            if (!string.IsNullOrEmpty(request.orderColumnName) && !string.IsNullOrEmpty(request.orderDir))
            {
                projects = await _projectRepository.OrderByField(projects, request.orderColumnName, request.orderDir == "asc");
                recordTotal = projects.Count();
                recordsFiltered = projects.Count();
            }
            var responseData = projects.Skip(request.Start).Take(request.Length).ToList();
            var mapping = _mapper.Map<IReadOnlyList<GetAllProjectVM>>(responseData);
            return new DataTableReponse<IReadOnlyList<GetAllProjectVM>>(mapping, recordsFiltered, recordTotal);
        }


    }
}
