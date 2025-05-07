using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Queries.PrintStudy
{
    public class GetPrintStudyDetailsByProjectIdQuery : IRequest<Response<PrintStudyDetailViewModel>>
    {
        public int ProjectId { get; set; }
    }

    public class GetPrintStudyDetilsByProjectIdQueryHandler : IRequestHandler<GetPrintStudyDetailsByProjectIdQuery, Response<PrintStudyDetailViewModel>>
    {
        private IPrintStudyRepositoryAsync _printStudyRepository;
        private IMapper _mapper;
        public GetPrintStudyDetilsByProjectIdQueryHandler(IPrintStudyRepositoryAsync printStudyRepository, IMapper mapper)
        {
            _printStudyRepository = printStudyRepository;
            _mapper = mapper;
        }

        public async Task<Response<PrintStudyDetailViewModel>> Handle(GetPrintStudyDetailsByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _printStudyRepository.GetAsync(x => x.ProjectId == request.ProjectId);
            if (data is null)
            {
                return new Response<PrintStudyDetailViewModel>(null,null);
            }
            var printStudyDetailViewModel = _mapper.Map<PrintStudyDetailViewModel>(data);
            return new Response<PrintStudyDetailViewModel>(printStudyDetailViewModel);
        }
    }
}
