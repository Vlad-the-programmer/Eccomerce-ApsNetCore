using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_Sustem.Models.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? CategoryName { get; set; } = null!;
        public string Brand { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime DateCreated { get; set; }
        public int SubcategoryId { get; set; }
        public string? Photo { get; set; } = default!;
        public string? OtherPhotos { get; set; } = default!;
        public string? About { get; set; } = default!;
        public string LongAbout { get; set; }
        public int? RatingSum { get; set; } = default!;
        public int? RatingVotes { get; set; } = default!;
    }
}
