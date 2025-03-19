using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_Sustem.Models.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
   
    }
}
