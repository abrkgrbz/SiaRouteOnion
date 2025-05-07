using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.User;

namespace Identity.DTOs
{
    public class UserResponseGroupedDepartmenDTO
    {
        public string DepartmanName { get; set; }
        public List<UserDto> Users { get; set; }
    }
}
