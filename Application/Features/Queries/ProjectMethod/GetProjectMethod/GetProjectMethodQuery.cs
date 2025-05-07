using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Queries.ProjectMethod.GetProjectMethod
{
    public class GetProjectMethodViewModel
    {
        public int Id { get; set; }
        public string MethodName { get; set; }
        public int Size { get; set; }
    }

    public class GetProjectMethodQuery:IRequest<Response<List<GetProjectMethodViewModel>>>
    {
        public int ProjectId { get; set; }
    }

    public class GetProjectMethodQueryHandler:IRequestHandler<GetProjectMethodQuery, Response<List<GetProjectMethodViewModel>>>
    {
        private readonly IProjectMethodsRepositoryAsync _projectMethodsRepositoryAsync;
        private readonly IMapper _mapper;
        public GetProjectMethodQueryHandler(IProjectMethodsRepositoryAsync projectMethodsRepositoryAsync, IMapper mapper)
        {
            _projectMethodsRepositoryAsync = projectMethodsRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<GetProjectMethodViewModel>>> Handle(GetProjectMethodQuery request, CancellationToken cancellationToken)
        {
            var data = await _projectMethodsRepositoryAsync.GetWhereList(x => x.ProjectId == request.ProjectId);
            var mapper = _mapper.Map<List<GetProjectMethodViewModel>>(data);
            return new Response<List<GetProjectMethodViewModel>>(mapper);
        }
    }
}
