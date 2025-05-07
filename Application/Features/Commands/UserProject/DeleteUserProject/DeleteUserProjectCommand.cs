using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Commands.UserProject.DeleteUserProject
{
    public class DeleteUserProjectCommand : IRequest<Response<bool>>
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; }
    }

    public class DeleteUserProjectCommandHandler : IRequestHandler<DeleteUserProjectCommand, Response<bool>>
    {
        private IUserProjectRepositoryAsync _userProjectRepository;

        public DeleteUserProjectCommandHandler(IUserProjectRepositoryAsync userProjectRepository)
        {
            _userProjectRepository = userProjectRepository;
        }

        public async Task<Response<bool>> Handle(DeleteUserProjectCommand request, CancellationToken cancellationToken)
        {
            var data = await _userProjectRepository.GetAsync(x =>
                x.ProjectId.Equals(request.ProjectId) && x.UserId.Equals(request.UserId));
            if (data is not null)
            {
                await _userProjectRepository.DeleteAsync(data, cancellationToken);
                return new Response<bool>(true);
            }
            return new Response<bool>(false, "Kullanıcı bulunamadı!");
        }
    }
}
