using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IPrintStudyRepositoryAsync:IGenericRepositoryAsync<PrintStudy>
    {
        Task<bool> IsUniqueProject(int projectId);
    }
}
