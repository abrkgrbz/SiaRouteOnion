using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Queries.Response.GetAllResponse
{
    public class GetAllResponseQuery:IRequest<Response<IEnumerable<Domain.Entities.Response>>>
    {
        public int projectId { get; set; }

    }
     

    public class GetAllResponseQueryHandler : IRequestHandler<GetAllResponseQuery, Response<IEnumerable<Domain.Entities.Response>>>
    {
        private readonly IResponseRepositoryAsync _responseRepository;
        public GetAllResponseQueryHandler(IResponseRepositoryAsync responseRepository)
        {
            _responseRepository = responseRepository;
        }
        public async Task<Response<IEnumerable<Domain.Entities.Response>>> Handle(GetAllResponseQuery request, CancellationToken cancellationToken)
        {
            var responses = await _responseRepository.GetWhereList(x => x.ProjectId == request.projectId);
            return new Response<IEnumerable<Domain.Entities.Response>>(responses);
        }
    }
}
