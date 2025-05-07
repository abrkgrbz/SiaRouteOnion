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

namespace Application.Features.Queries.ProjectNote
{
  
    public class GetAllProjectNoteByProjectQuery : IRequest<Response<Dictionary<NoteType, List<GetAllProjectNoteViewModel>>>>
    {
        public int ProjectId { get; set; }
    }

    public class GetAllProjectNoteByProjectQueryHandler : IRequestHandler<GetAllProjectNoteByProjectQuery, Response<Dictionary<NoteType, List<GetAllProjectNoteViewModel>>>>
    {
        private readonly IProjectNoteRepositoryAsync _projectNoteRepositoryAsync;
        private readonly IMapper _mapper;
        public GetAllProjectNoteByProjectQueryHandler(IProjectNoteRepositoryAsync projectNoteRepositoryAsync, IMapper mapper)
        {
            _projectNoteRepositoryAsync = projectNoteRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<Dictionary<NoteType, List<GetAllProjectNoteViewModel>>>> Handle(GetAllProjectNoteByProjectQuery request, CancellationToken cancellationToken)
        {
            var projectNote = await _projectNoteRepositoryAsync.GetWhereList(x => x.ProjectId == request.ProjectId);
            if (projectNote is not null && projectNote.Any())
            {
                var mappingProfile = _mapper.Map<List<GetAllProjectNoteViewModel>>(projectNote);

                // NoteType enum'ına göre gruplama yapılıyor
                var gruplanmisNotlar = mappingProfile.GroupBy(x => x.NoteType)
                    .ToDictionary(g => g.Key, g => g.ToList());

                return new Response<Dictionary<NoteType, List<GetAllProjectNoteViewModel>>>(gruplanmisNotlar);
            }
            throw new ApiException("Projeye ait not bulunamadı");
        }
    }
}
