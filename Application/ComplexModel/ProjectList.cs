using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ComplexModel
{
    public class ProjectList
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectHeader { get; set; }
        public int ProjectStatus { get; set; }
        public DateTime? PlanlananSahaBitis { get; set; }
        public DateTime? PlanlananRaporTeslim { get; set; }
        public List<string>? ProjectOfficers { get; set; }

    }
}
