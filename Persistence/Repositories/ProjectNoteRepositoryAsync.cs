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
    public class ProjectNoteRepositoryAsync:GenericRepositoryAsync<ProjectNotes>,IProjectNoteRepositoryAsync
    {
        private readonly DbSet<ProjectNotes> _projectNotes;
        public ProjectNoteRepositoryAsync(SiaRouteDbContext dbContext) : base(dbContext)
        {
            _projectNotes = dbContext.Set<ProjectNotes>();
        }
    }
}
