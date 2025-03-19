using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceRestApi.Models.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
