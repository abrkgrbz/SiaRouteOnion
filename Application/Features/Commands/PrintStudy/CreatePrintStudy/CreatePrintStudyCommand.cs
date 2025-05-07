using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Commands.PrintStudy.CreatePrintStudy
{
    public class CreatePrintStudyCommand : IRequest<Response<bool>>
    {
        public string ProjectCode { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int ProjectId { get; set; }
    }

    public class CreatePrintStudyCommandHandler : IRequestHandler<CreatePrintStudyCommand, Response<bool>>
    {
        private IPrintStudyRepositoryAsync _printStudyRepository;
        private IMapper _mapper;
        public CreatePrintStudyCommandHandler(IPrintStudyRepositoryAsync printStudyRepository, IMapper mapper)
        {
            _printStudyRepository = printStudyRepository;
            _mapper = mapper;
        }

        public async Task<Response<bool>> Handle(CreatePrintStudyCommand request, CancellationToken cancellationToken)
        {
            var mapping = _mapper.Map<Domain.Entities.PrintStudy>(request);
            var result = await _printStudyRepository.AddAsync(mapping,cancellationToken);
            return new Response<bool>(true);
        }
    }
}
