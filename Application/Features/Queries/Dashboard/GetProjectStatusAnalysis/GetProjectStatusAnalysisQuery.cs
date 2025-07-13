using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Queries.Dashboard.GetProjectStatusAnalysis
{
    public class GetProjectStatusAnalysisQuery:IRequest<List<GetProjectStatusAnalysisVM>>
    {
    }
    public class GetProjectStatusAnalysisQueryHandler : IRequestHandler<GetProjectStatusAnalysisQuery, List<GetProjectStatusAnalysisVM>>
    {
        private readonly IProjectRepositoryAsync _projectRepository;

        public GetProjectStatusAnalysisQueryHandler(IProjectRepositoryAsync projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<List<GetProjectStatusAnalysisVM>> Handle(GetProjectStatusAnalysisQuery request, CancellationToken cancellationToken)
        {
            var result = _projectRepository
                .GetAll(false)
                .GroupBy(p => p.ProjectStatus)
                .Select(g => new GetProjectStatusAnalysisVM()
                {
                    Status = g.Key,
                    Adet = g.Count()
                }).ToList();

            return result;
        }
    }
}
