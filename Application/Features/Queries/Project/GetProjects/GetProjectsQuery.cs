using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Queries.Project.GetProjects
{
    public class GetProjectsQuery : IRequest<Response<List<GetProjectsVM>>>
    {
    }

    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, Response<List<GetProjectsVM>>>
    {

        private readonly IProjectRepositoryAsync _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectsQueryHandler(IMapper mapper, IProjectRepositoryAsync projectRepository)
        {
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<Response<List<GetProjectsVM>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var data = await _projectRepository.GetAllProjectsComplex();
            var mapping = _mapper.Map<List<GetProjectsVM>>(data);
            return new Response<List<GetProjectsVM>>(mapping);
        }
    }
}
