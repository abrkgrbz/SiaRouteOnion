using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Commands.ProjectNote
{
    public class DeleteProjectNoteCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteProjectNoteCommandHandler : IRequestHandler<DeleteProjectNoteCommand, Response<bool>>
    {
        private readonly IProjectNoteRepositoryAsync _projectNoteRepositoryAsync;

        public DeleteProjectNoteCommandHandler(IProjectNoteRepositoryAsync projectNoteRepositoryAsync)
        {
            _projectNoteRepositoryAsync = projectNoteRepositoryAsync;
        }

        public async Task<Response<bool>> Handle(DeleteProjectNoteCommand request, CancellationToken cancellationToken)
        {
            var data = await _projectNoteRepositoryAsync.GetAsync(x => x.Id == request.Id);
            var deletedDataResult = await _projectNoteRepositoryAsync.DeleteAsync(data,cancellationToken);
            return new Response<bool>(deletedDataResult);
        }
    }
}


