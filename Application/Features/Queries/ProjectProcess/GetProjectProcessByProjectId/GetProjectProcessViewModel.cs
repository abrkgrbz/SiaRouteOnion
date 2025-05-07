using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Queries.ProjectProcess.GetProjectProcessByProjectId
{
    public class GetProjectProcessViewModel
    {
        public string ProcessName { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? RealizedDate { get; set; }
        public string Status { get; set; }
    }
}
