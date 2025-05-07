using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class Response:AuditableBaseEntity,ICloneable
    {
        public string? ParentQuestionName { get; set; }
        public string? QuestionName { get; set; }
        public string? ResponseText { get; set; }
        public int? ResponseValue { get; set; }

        public int ProjectId { get; set; } // Foreign Key
        public Project Project { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
