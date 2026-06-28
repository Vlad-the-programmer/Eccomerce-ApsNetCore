using EcommerceRestApi.Helpers.Data.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models
{
    public class Notification : EntityBase
    {
        public string? UserId { get; set; }
        public int? CustomerId { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        [InverseProperty("Notifications")]
        public virtual Customer? Customer { get; set; }
        [InverseProperty("Notifications")]
        public virtual ApplicationUser? User { get; set; }
    }

}
