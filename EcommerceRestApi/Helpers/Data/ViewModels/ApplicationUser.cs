using EcommerceRestApi.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class ApplicationUser: IdentityUser
    {
       [Display(Name = "Full Name")]
       public string FullName { get; set; }

       [Display(Name = "Role")]
       public string Role { get; set; }
        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public DateTime DateDeleted { get; set; } = DateTime.Now;


        [InverseProperty("User")]
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
