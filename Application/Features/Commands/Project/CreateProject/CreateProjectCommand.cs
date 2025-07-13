using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Commands.Project.CreateProject
{
    public class CreateProjectCommand:IRequest<Response<bool>>
    {
        public string ProjectName { get; set; }
        public string ProjectHeader { get; set; }
        public string CustomerName { get; set; }
        public string ProjectSector { get; set; } 
        public Dictionary<string, string>  ProjectSizes { get; set; }
        public string ProjectManager { get; set; }
        public int ProjectStatus { get; set; }
        public string ProjectNotes { get; set; }
        public DateTime? PlanlananSoruFormuTeslim { get; set; }
        public DateTime? PlanlananRaporTeslim { get; set; }
        public DateTime? PlanlananScriptTeslim { get; set; }
        public DateTime? PlanlananScriptKontrol { get; set; }
        public DateTime? PlanlananScriptRevizyon { get; set; }
        public DateTime? PlanlananSahaBaslangic { get; set; }
        public DateTime? PlanlananSahaBitis { get; set; }
        public DateTime? PlanlananKodlamaTeslim { get; set; }
        public DateTime? PlanlananTablolamaTeslim { get; set; }
        public List<string> ProjectOfficers { get; set; }
        public List<int> ScmId { get; set; }

    }

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Response<bool>>
    {
        private readonly IProjectRepositoryAsync _projectRepositoryAsync;
        private readonly IProjectOfficersRepositoryAsync _projectOfficersRepositoryAsync;
        private readonly IProjectProcessRepositoryAsync _projectProcessRepositoryAsync;
        private readonly IProjectSCMRepositoryAsync _projectSCMRepositoryAsync;
        private readonly IProjectNoteRepositoryAsync _projectNoteRepository;
        private readonly IProjectMethodsRepositoryAsync _projectMethodsRepositoryAsync;
        private IMapper _mapper;
        public CreateProjectCommandHandler(IProjectRepositoryAsync projectRepositoryAsync, IMapper mapper, IProjectProcessRepositoryAsync projectProcessRepositoryAsync, IProjectOfficersRepositoryAsync projectOfficersRepositoryAsync, IProjectSCMRepositoryAsync projectScmRepositoryAsync, IProjectMethodsRepositoryAsync projectMethodsRepositoryAsync, IProjectNoteRepositoryAsync projectNoteRepository)
        {
            _projectRepositoryAsync = projectRepositoryAsync;
            _mapper = mapper;
            _projectProcessRepositoryAsync = projectProcessRepositoryAsync;
            _projectOfficersRepositoryAsync = projectOfficersRepositoryAsync;
            _projectSCMRepositoryAsync = projectScmRepositoryAsync;
            _projectMethodsRepositoryAsync = projectMethodsRepositoryAsync;
            _projectNoteRepository = projectNoteRepository;
        }

        public async Task<Response<bool>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = new Domain.Entities.Project()
                {
                    IsActive = true,
                    ProjectName = request.ProjectName, 
                    CustomerName = request.CustomerName,
                    ProjectManager = request.ProjectManager,
                    ProjectHeader = request.ProjectHeader,
                    ProjectSector = request.ProjectSector,
                    ProjectStatus = request.ProjectStatus

                };
                var newProject = await _projectRepositoryAsync.AddAsync(project, cancellationToken);
                if (newProject is not null)
                {
                    if (request.ProjectOfficers != null && request.ProjectOfficers.Any())
                    {
                        var projectOfficers = request.ProjectOfficers.Select(officerId =>
                            new Domain.Entities.ProjectOfficers()
                            {
                                ProjectId = newProject.Id,
                                UserId = officerId
                            }).ToList();
                        await _projectOfficersRepositoryAsync.AddRangeAsync(projectOfficers, cancellationToken);
                    }

                    if (request.ScmId != null && request.ScmId.Any())
                    {
                        var scms = request.ScmId.Select(scm => new ProjectSCM()
                        {
                            ProjectId = newProject.Id,
                            SCMId = scm
                        }).ToList();
                        await _projectSCMRepositoryAsync.AddRangeAsync(scms, cancellationToken);
                    }

                    if (request.ProjectSizes != null && request.ProjectSizes.Any())
                    {
                        var projectMethods = request.ProjectSizes.Select(projectMethods => new ProjectMethods()
                        {
                            ProjectId = newProject.Id,
                            MethodName = projectMethods.Key,
                            Size = Convert.ToInt32(projectMethods.Value),


                        }).ToList();
                        await _projectMethodsRepositoryAsync.AddRangeAsync(projectMethods, cancellationToken);
                    }

                    if (request.ProjectNotes != null && request.ProjectNotes.Any())
                    {
                        var projectNotes = new ProjectNotes()
                            { ProjectId = newProject.Id, Note = request.ProjectNotes,NoteType = 1,NoteCategory = "Not"};
                        await _projectNoteRepository.AddAsync(projectNotes, cancellationToken);
                    }

                    var result=await _projectProcessRepositoryAsync.AddAsync(new Domain.Entities.ProjectProcess()
                    {
                        ProjectId = newProject.Id,
                        PlanlananSoruFormuTeslim = ConvertToUtc(request.PlanlananSoruFormuTeslim),
                        PlanlananRaporTeslim = ConvertToUtc(request.PlanlananRaporTeslim),
                        PlanlananKodlamaTeslim = ConvertToUtc(request.PlanlananKodlamaTeslim),
                        PlanlananSahaBaslangic = ConvertToUtc(request.PlanlananSahaBaslangic),
                        PlanlananSahaBitis = ConvertToUtc(request.PlanlananSahaBitis),
                        PlanlananScriptKontrol = ConvertToUtc(request.PlanlananScriptKontrol),
                        PlanlananScriptRevizyon = ConvertToUtc(request.PlanlananScriptRevizyon),
                        PlanlananScriptTeslim = ConvertToUtc(request.PlanlananScriptTeslim),
                        PlanlananTablolamaTeslim = ConvertToUtc(request.PlanlananTablolamaTeslim),
                    }, cancellationToken);
                    return new Response<bool>(true, "Proje Oluşturuldu");
                }
            }
            catch (Exception e)
            { 
                throw new ApiException("Proje oluşturulamadı.");
            }
            throw new ApiException("Proje oluşturulamadı.");
        }


        private DateTime? ConvertToUtc(DateTime? date)
        {
            if (date.HasValue)
            {
                return DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
            }

            return null;
        }
    }
}
