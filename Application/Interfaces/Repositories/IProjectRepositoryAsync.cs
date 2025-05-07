using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ComplexModel;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IProjectRepositoryAsync:IGenericRepositoryAsync<Project>
    {
        Task<bool> IsUniqueProject(string projectCode);
        Task<bool> IsEqualsProjectName(string projectName, int projectId);
        Task<IQueryable<ProjectList>> GetAllProjectsComplex();
    }
}
