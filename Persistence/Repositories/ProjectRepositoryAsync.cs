using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ComplexModel;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories
{
    public class ProjectRepositoryAsync : GenericRepositoryAsync<Project>, IProjectRepositoryAsync
    {
        private readonly DbSet<Project> _projects; 
        private readonly IUserService _userService;
        public ProjectRepositoryAsync(SiaRouteDbContext dbContext, IUserService userService) : base(dbContext)
        {
            _userService = userService;
            _projects = dbContext.Set<Project>();
        }

        public Task<bool> IsUniqueProject(string projectCode)
        {
            return _projects.AllAsync(x => x.ProjectName != projectCode);
        }

         

        public async Task<bool> IsEqualsProjectName(string projectName,int projectId)
        { 
            var project =await _projects.FirstOrDefaultAsync(x => x.Id == projectId);
            return project.ProjectName == projectName;
        }

        public async Task<IQueryable<ProjectList>> GetAllProjectsComplex()
        { 
            var users =await _userService.GetAllUser();
            var data = _projects
                .Include(x => x.ProjectOfficers)
                .Include(x => x.ProjectProcess)
                .AsNoTracking()
                .ToList();
            List<ProjectList> projectLists = new();
            foreach (var item in data)
            {
                var firstNameInitials = item.ProjectOfficers
                    .Join(users, po => po.UserId, u => u.Id, (po, u) => new { u.Name,u.LastName })
                    .Select(x => x.Name.Substring(0, 1)+x.LastName.Substring(0,1)) 
                    .ToList();

                projectLists.Add(new ProjectList()
                {
                    PlanlananRaporTeslim = item.ProjectProcess.PlanlananRaporTeslim,
                    PlanlananSahaBitis = item.ProjectProcess.PlanlananSahaBitis,
                    ProjectHeader = item.ProjectHeader,
                    ProjectName = item.ProjectName,
                    ProjectId = item.Id,
                    ProjectStatus = item.ProjectStatus,
                    ProjectOfficers = firstNameInitials
                });
            }

            return projectLists.AsQueryable();
        }

       
    }
}
