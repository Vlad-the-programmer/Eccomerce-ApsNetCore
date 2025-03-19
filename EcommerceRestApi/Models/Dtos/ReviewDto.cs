using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_Sustem.Models.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public string Username { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public bool IsActive { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = null!;
        public DateTime DateCreated { get; set; }
    }
}
