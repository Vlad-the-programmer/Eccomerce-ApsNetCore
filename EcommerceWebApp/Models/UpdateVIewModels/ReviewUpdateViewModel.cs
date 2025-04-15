using EcommerceWebApp.Models.UpdateViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebApp.Models
{
    public class ReviewUpdateViewModel : BaseUpdateViewModel
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Column(TypeName = "text")]
        [StringLength(2000, ErrorMessage = "Review text can't be longer than 2000 characters.")]
        public string ReviewText { get; set; } = null!;
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

    }
}
