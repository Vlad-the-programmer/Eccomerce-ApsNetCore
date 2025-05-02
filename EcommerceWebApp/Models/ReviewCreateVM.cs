using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebApp.Models
{
    public class ReviewCreateVM : BaseViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Review text is required.")]
        [Column(TypeName = "text")]
        [StringLength(2000, ErrorMessage = "Review text can't be longer than 2000 characters.")]
        public string ReviewText { get; set; } = null!;

    }
}
