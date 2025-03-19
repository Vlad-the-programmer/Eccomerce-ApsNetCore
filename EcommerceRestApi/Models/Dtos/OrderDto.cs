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
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public int? OrderItemsCount { get; set; }

        public string Status { get; set; } = null!;
        public string FullName { get; set; } = null;

        public DateTime OrderDate { get; set; }
        public int? CustomerId { get; set; } = null;
        public bool? IsPaid { get; set; } = null;
        public string DeliveryMethod { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
