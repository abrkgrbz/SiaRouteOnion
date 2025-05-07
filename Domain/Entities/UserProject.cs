using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class UserProject:AuditableBaseEntity
    {
        public string UserId { get; set; }

        public int ProjectId { get; set; }
         
    }
}