using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class PrintStudy:AuditableBaseEntity
    {
        public string ProjectCode { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int ProjectId { get; set; } // Foreign Key
        public Project Project { get; set; }
    }
}
