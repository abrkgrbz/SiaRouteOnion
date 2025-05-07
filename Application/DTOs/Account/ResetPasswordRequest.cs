using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Account
{
    public class ResetPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Id { get; set; }
        [Required]
        [MinLength(6)]
        public string CurrentPassword { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; } 
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } 
    }
}
