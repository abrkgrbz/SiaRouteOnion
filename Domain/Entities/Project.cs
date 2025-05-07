using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Project: AuditableBaseEntity
    {
        public string ProjectName { get; set; }
        public int ProjectStatus { get; set; }
        public string ProjectHeader { get; set; }
        public string CustomerName { get; set; }
        public string ProjectSector { get; set; } 
        public string ProjectManager { get; set; }
        public string? ProjectLink { get; set; }
        public int? SurveyId { get; set; }
        public bool IsActive { get; set; } 
        public ICollection<Question> Questions { get; set; }
        public ICollection<Response> Responses { get; set; }
        public ICollection<PrintStudy> PrintStudies { get; set; }
        public ProjectProcess ProjectProcess { get; set; }
        public ICollection<ProjectOfficers> ProjectOfficers { get; set; }
        public ICollection<ProjectNotes> ProjectNotes { get; set; }
        public ICollection<ProjectSCM> ProjectScms { get; set; }
        public ICollection<ProjectMethods> ProjectMethods { get; set; }
    }
}
