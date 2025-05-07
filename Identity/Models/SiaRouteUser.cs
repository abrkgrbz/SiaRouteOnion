using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Account;
using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class SiaRouteUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
          
        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}
