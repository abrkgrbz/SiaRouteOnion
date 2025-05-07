using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Queries.Scm.GetAllScm
{
    public class GetAllScmQuery:IRequest<Response<List<GetAllScmViewModel>>>
    {
        
    }

    public class GetAllScmQueryHandler : IRequestHandler<GetAllScmQuery, Response<List<GetAllScmViewModel>>>
    {
        private readonly ISCMRepositoryAsync _scmRepositoryAsync;
        private readonly IMapper _mapper;
        public GetAllScmQueryHandler(ISCMRepositoryAsync scmRepositoryAsync, IMapper mapper)
        {
            _scmRepositoryAsync = scmRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<GetAllScmViewModel>>> Handle(GetAllScmQuery request, CancellationToken cancellationToken)
        {
            var data = _scmRepositoryAsync.GetAll(false).ToList();
            var mappingProfile = _mapper.Map<List<GetAllScmViewModel>>(data);
            return new Response<List<GetAllScmViewModel>>() { Data = mappingProfile };
        }
    }
}
