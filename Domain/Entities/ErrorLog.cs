using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities
{
    public class ErrorLog:BaseEntity
    { 
        public DateTime ErrorDateTime { get; set; }
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Title { get; set; }
        public string RequestUrl { get; set; }
        public string Method { get; set; }
        public string UserIP { get; set; }
    }
}
