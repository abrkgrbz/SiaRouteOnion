using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Wrappers;
using Identity.DTOs;
using MediatR;

namespace Application.Features.Queries.User.GetAllUserList
{
    public class GetAllUserListQuery:IRequest<Response<List<UserResponseGroupedDepartmenDTO>>>
    {
    }

    public class GetAllUserListQueryHandler : IRequestHandler<GetAllUserListQuery, Response<List<UserResponseGroupedDepartmenDTO>>>
    {
        private readonly IUserService _userService;

        public GetAllUserListQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Response<List<UserResponseGroupedDepartmenDTO>>> Handle(GetAllUserListQuery request, CancellationToken cancellationToken)
        {
            var data =await _userService.GetAllUserGroupByDepartman();
            return new Response<List<UserResponseGroupedDepartmenDTO>>() { Data = data };
        }
    }
}
