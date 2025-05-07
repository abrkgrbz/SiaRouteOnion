using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; } 
        public ICollection<SiaRouteUser> Users { get; set; }
    }

}
