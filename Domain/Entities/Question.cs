using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class Question: AuditableBaseEntity,ICloneable
    { 
        public string? CornerLabel { get; set; }
        public string? Footer { get; set; }
        public string? GridFooter { get; set; }
        public string? GridHeader { get; set; }
        public string? Header1 { get; set; }
        public string? Header2 { get; set; }
        public bool? IsMultiple { get; set; }
        public string? QuestionDirection { get; set; }
        public string? QuestionName { get; set; }
        public short? QuestionOrder { get; set; }
        public string? QuestionText { get; set; }
        public string? QuestionType { get; set; }
        public string? ReportText { get; set; }

        public int ProjectId { get; set; } // Foreign Key
        public Project Project { get; set; }
        public object Clone()
        {
           return this.MemberwiseClone();
        }
    }
}
