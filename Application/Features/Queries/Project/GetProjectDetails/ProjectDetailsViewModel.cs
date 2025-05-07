using Application.Features.Queries.ProjectMethod.GetProjectMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Queries.Project.GetProjectDetails
{
	public class ProjectDetailsViewModel
	{
		public int Id { get; set; }
		public string ProjectName { get; set; }
		public int ProjectStatus { get; set; }
		public string CreatedBy { get; set; }
        public string ProjectSector { get; set; }
        public string ProjectMethod { get; set; }
        public string CustomerName { get; set; }
        public string ProjectHeader { get; set; }
        public string? ProjectLink { get; set; }
        public int? SurveyId { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive { get; set; }

		public ProjectProcessesDetailViewModel ProcessDetails { get; set; }
        public List<GetProjectMethodViewModel> GetProjectMethodViewModels { get; set; }
	}
}
