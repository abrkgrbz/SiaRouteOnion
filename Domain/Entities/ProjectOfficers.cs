using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class ProjectOfficers : AuditableBaseEntity
    {
        public string UserId { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
