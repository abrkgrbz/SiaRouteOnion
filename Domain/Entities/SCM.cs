using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class SCM:BaseEntity
    { 
        public string SCMName { get; set; }
        public bool Active { get; set; }
        public ICollection<ProjectSCM> ProjectScms { get; set; }
    }
}
