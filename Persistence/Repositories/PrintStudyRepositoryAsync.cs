using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class PrintStudyRepositoryAsync:GenericRepositoryAsync<PrintStudy>, IPrintStudyRepositoryAsync
    {
        private readonly DbSet<PrintStudy> _printStudies;
        public PrintStudyRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        {
            _printStudies = dbContext.Set<PrintStudy>();
        }

       
        public Task<bool> IsUniqueProject(int projectId)
        {
            return _printStudies.AllAsync(x => x.ProjectId != projectId);
        }
    }
}
