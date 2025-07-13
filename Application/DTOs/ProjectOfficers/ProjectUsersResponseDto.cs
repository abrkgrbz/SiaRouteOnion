using Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProjectOfficers
{
    public class ProjectUsersResponseDto
    {
        public List<UserResponse> Users { get; set; } = new List<UserResponse>();
        public int TotalCount { get; set; }
    }
}
