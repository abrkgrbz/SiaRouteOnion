using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Commands.ProjectNote
{
    public class CreateProjectNoteCommand:IRequest<Response<bool>>
    {
        public NoteType NoteType { get; set; }
        public string Note { get; set; }
        public int ProjectId { get; set; }
    }

    public class CreateProjectNoteCommandHandler : IRequestHandler<CreateProjectNoteCommand,  Response<bool>>
    {
        private readonly IProjectNoteRepositoryAsync _projectNoteRepositoryAsync;
        private readonly IMapper _mapper;
        public CreateProjectNoteCommandHandler(IProjectNoteRepositoryAsync projectNoteRepositoryAsync, IMapper mapper)
        {
            _projectNoteRepositoryAsync = projectNoteRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<bool>> Handle(CreateProjectNoteCommand request, CancellationToken cancellationToken)
        {
            var mappingProfile = _mapper.Map<Domain.Entities.ProjectNotes>(request);
            mappingProfile.NoteCategory = request.NoteType.ToString();
            var data =await _projectNoteRepositoryAsync.AddAsync(mappingProfile, cancellationToken);
            if (data is not null)
            {
                return new Response<bool>(true,"Proje Notu başarıyla oluşturuldu");
            }

            throw new ApiException("Not oluşturma başarısız");
        }
    }
}
