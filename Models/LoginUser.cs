using System;
using System.ComponentModel.DataAnnotations;

namespace loginAndRegistration.Models
{
    public class LoginUser
    {
        [Key]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}