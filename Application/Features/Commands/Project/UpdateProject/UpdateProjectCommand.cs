using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using MediatR;

namespace Application.Features.Commands.Project.UpdateProject
{
    public class UpdateProjectCommand:IRequest<Response<bool>>
    {
        public int ProjectId { get; set; }
        public int ProjectStatus { get; set; }
        public bool IsActive { get; set; }
        public string ProjectLink { get; set; }
        public int SurveyId { get; set; }

    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Response<bool>>
    {
        private IProjectRepositoryAsync _projectRepositoryAsync; 
        public UpdateProjectCommandHandler(IProjectRepositoryAsync projectRepositoryAsync )
        {
            _projectRepositoryAsync = projectRepositoryAsync; 
        }

        public async Task<Response<bool>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var data = await _projectRepositoryAsync.GetByIdAsync(request.ProjectId); 
            data.IsActive=request.IsActive;
            data.ProjectStatus=request.ProjectStatus;
            data.ProjectLink = request.ProjectLink;
            data.SurveyId = request.SurveyId;
            await _projectRepositoryAsync.UpdateAsync(data,cancellationToken);
            return new Response<bool>(true, "Proje güncelleme işlemi başarıyla gerçekleştirildi");
        }
    }
}
