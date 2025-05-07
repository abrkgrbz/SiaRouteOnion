using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Commands.User.ApproveUser
{
    public class ApproveUserCommand : IRequest<Response<bool>>
    {
        public string userId { get; set; }
    }


    public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommand, Response<bool>>
    {
        private IUserService _userService;

        public ApproveUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Response<bool>> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
        {
            bool approveUser = await _userService.ApproveUser(request.userId);
            if (approveUser)
            {
                return new Response<bool>(true, "Kullanıcı onaylandı");
            }

            return new Response<bool>(false, "Kullanıcı onayı başarısız");
        }
    }

}
