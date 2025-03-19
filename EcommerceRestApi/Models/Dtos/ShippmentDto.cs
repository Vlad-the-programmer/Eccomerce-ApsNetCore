using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceRestApi.Models.Dtos
{
    public class ShipmentDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public string? CustomerName { get; set; } = null!;
        public string? CountryName { get; set; }
        public Address Address { get; set; } = null!;
        public DateTime ShipmentDate { get; set; }
        public DateTime EstimatedArrivalDate { get; set; }
        public DateTime DateCreated { get; set; }
        public string DeliveryMethodName { get; set; } = string.Empty; // Additional property to include delivery method details

    }
}
