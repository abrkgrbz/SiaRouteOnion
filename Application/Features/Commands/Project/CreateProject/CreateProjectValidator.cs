using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using FluentValidation;

namespace Application.Features.Commands.Project.CreateProject
{
    public class CreateProjectValidator:AbstractValidator<CreateProjectCommand>
    {
        private IProjectRepositoryAsync _projectRepository;
        public CreateProjectValidator(IProjectRepositoryAsync projectRepository)
        {
            _projectRepository = projectRepository;
            RuleFor(p => p.ProjectName)
                .NotEmpty().WithMessage("Proje Adı Zorunludur");

            RuleFor(x => x.ProjectName)
                .MustAsync(IsUniqueProject).WithMessage("Bu Proje sistemde tanımlı!");
        }

        private async Task<bool> IsUniqueProject(string projectCode, CancellationToken cancellationToken)
        {
            return await _projectRepository.IsUniqueProject(projectCode);
        }
    }
}
