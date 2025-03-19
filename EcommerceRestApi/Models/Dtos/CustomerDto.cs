using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceRestApi.Models.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? CountryName { get; set; } = null;
        public string FullName { get; set; } = null!;
        public Address? Address { get; set; } = null;
        public string Email { get; set; } = null!;
        public string Nip { get; set; } = null!;
        public int? Points { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
