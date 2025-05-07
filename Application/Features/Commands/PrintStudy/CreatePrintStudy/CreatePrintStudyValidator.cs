using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using FluentValidation;

namespace Application.Features.Commands.PrintStudy.CreatePrintStudy
{
    public class CreatePrintStudyValidator:AbstractValidator<CreatePrintStudyCommand>
    {
        private readonly IPrintStudyRepositoryAsync _printStudyRepository;
        private readonly IProjectRepositoryAsync _projectRepository;    

        public CreatePrintStudyValidator(IPrintStudyRepositoryAsync printStudyRepository, IProjectRepositoryAsync projectRepository)
        {
            _printStudyRepository = printStudyRepository;
            _projectRepository = projectRepository;

            RuleFor(x => x.ProjectCode)
                .MustAsync((command, projectCode, cancellationToken) => IsEqualsProjectName(command.ProjectCode, command.ProjectId, cancellationToken)).WithMessage("Proje adları uyuşmamakta!");

            RuleFor(x => x.ProjectId)
                .MustAsync(IsUniqueProject).WithMessage("Bu PrintStudy dosyası sistemde yüklü!");

         
        }

        private async Task<bool> IsUniqueProject(int projectCode,CancellationToken cancellationToken)
        {
            return await _printStudyRepository.IsUniqueProject(projectCode);
        }

        private async Task<bool> IsEqualsProjectName(string projectName,int projectId, CancellationToken cancellationToken)
        {
            return await _projectRepository.IsEqualsProjectName(projectName,projectId);
        }

    }
}
