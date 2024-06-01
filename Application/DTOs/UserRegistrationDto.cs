using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserRegistrationDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public UserType Role { get; set; }
        public string Password { get; set; }
    }
}
