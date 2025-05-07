using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Commands.Project.DeleteProject
{
    public class DeleteProjectCommand : IRequest<Response<bool>>
    {
        public int ProjectId { get; set; }

    }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Response<bool>>
    {
        private readonly IProjectRepositoryAsync _projectRepository;

        public DeleteProjectCommandHandler(IProjectRepositoryAsync projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Response<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId);
            await _projectRepository.DeleteAsync(project, cancellationToken);
            return new Response<bool>(true);
        }
    }
}