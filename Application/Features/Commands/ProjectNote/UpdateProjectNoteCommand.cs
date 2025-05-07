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
    public class UpdateProjectNoteCommand:IRequest<Response<bool>>
    {
        public int Id { get; set; }
        public string Note { get; set; }
    }

    public class UpdateProjectNoteCommandHandler : IRequestHandler<UpdateProjectNoteCommand, Response<bool>>
    {
        private readonly IProjectNoteRepositoryAsync _projectNoteRepositoryAsync;

        public UpdateProjectNoteCommandHandler(IProjectNoteRepositoryAsync projectNoteRepositoryAsync)
        {
            _projectNoteRepositoryAsync = projectNoteRepositoryAsync;
        }

        public async Task<Response<bool>> Handle(UpdateProjectNoteCommand request, CancellationToken cancellationToken)
        {
            var data = await _projectNoteRepositoryAsync.GetAsync(x => x.Id == request.Id);
            data.Note=request.Note;
            var updatedProjecNote = await _projectNoteRepositoryAsync.UpdateAsync(data, cancellationToken);
            return new Response<bool>(updatedProjecNote);
        }
    }
}
