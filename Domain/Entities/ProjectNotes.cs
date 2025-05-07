using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class ProjectNotes:AuditableBaseEntity
    {
        public string Note { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public int NoteType { get; set; }
        public string NoteCategory { get; set; }
    }

    
}
