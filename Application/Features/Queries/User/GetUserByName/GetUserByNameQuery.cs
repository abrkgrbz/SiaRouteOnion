using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Queries.User.GetUserByName
{
    public class GetUserByNameQuery:IRequest<Response<List<GetUserByNameViewModel>>>
    {
        public string Search { get; set; }
    }

    public class GetUserByNameQueryHandler : IRequestHandler<GetUserByNameQuery, Response<List<GetUserByNameViewModel>>>
    {
        private IUserService _userService;
        private IMapper _mapper;
        public GetUserByNameQueryHandler(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }


        public async Task<Response<List<GetUserByNameViewModel>>> Handle(GetUserByNameQuery request, CancellationToken cancellationToken)
        {
            var data = _userService.GetUserResponses(request.Search);
            var mapper= _mapper.Map<List<GetUserByNameViewModel>>(data);
            return new Response<List<GetUserByNameViewModel>>(mapper);
        }
    }

}
