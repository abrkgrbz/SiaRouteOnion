using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class ProjectSCM:BaseEntity
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int SCMId { get; set; }
        public SCM Scm { get; set; }
    }
}
