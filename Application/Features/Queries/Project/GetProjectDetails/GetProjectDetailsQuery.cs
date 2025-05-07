using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Queries.ProjectMethod.GetProjectMethod;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Queries.Project.GetProjectDetails
{
	public class GetProjectDetailsQuery:IRequest<Response<ProjectDetailsViewModel>>
	{
		public int Id { get; set; }
	}

	public class GetProjectDetailsQueryHandler : IRequestHandler<GetProjectDetailsQuery, Response<ProjectDetailsViewModel>>
    {
        private IProjectProcessRepositoryAsync _projectProcessRepositoryAsync;
		private IProjectRepositoryAsync _projectRepository;
        private IProjectMethodsRepositoryAsync _projectMethodsRepositoryAsync;
		private IMapper _mapper;
		public GetProjectDetailsQueryHandler(IProjectRepositoryAsync projectRepository, IMapper mapper, IProjectProcessRepositoryAsync projectProcessRepositoryAsync, IProjectMethodsRepositoryAsync projectMethodsRepositoryAsync)
		{
			_projectRepository = projectRepository;
			_mapper = mapper;
            _projectProcessRepositoryAsync = projectProcessRepositoryAsync;
            _projectMethodsRepositoryAsync = projectMethodsRepositoryAsync;
        }

		public async Task<Response<ProjectDetailsViewModel>> Handle(GetProjectDetailsQuery request, CancellationToken cancellationToken)
		{
			var projectData= await _projectRepository.GetByIdAsync(request.Id);
            var projectMethods = await _projectMethodsRepositoryAsync.GetWhereList(x => x.ProjectId == request.Id);
            var projectProcess = await _projectProcessRepositoryAsync.GetAsync(x => x.ProjectId == projectData.Id); 
            var projectDetailsViewModel = _mapper.Map<ProjectDetailsViewModel>(projectData); 
            projectDetailsViewModel.ProcessDetails = _mapper.Map<ProjectProcessesDetailViewModel>(projectProcess);
            projectDetailsViewModel.GetProjectMethodViewModels =
                _mapper.Map<List<GetProjectMethodViewModel>>(projectMethods);
            return new Response<ProjectDetailsViewModel>(projectDetailsViewModel);
		}
	}
}
