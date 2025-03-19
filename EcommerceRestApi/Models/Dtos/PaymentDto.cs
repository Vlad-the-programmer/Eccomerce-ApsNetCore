using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceRestApi.Models.Dtos
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? PaymentMethodId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
