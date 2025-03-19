using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

public partial class Review : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Product ID is required.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Customer ID is required.")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Rating is required.")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Review text is required.")]
    [Column(TypeName = "text")]
    [StringLength(1000, ErrorMessage = "Review text cannot exceed 1000 characters.")]
    public string ReviewText { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("Reviews")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("Reviews")]
    public virtual Product Product { get; set; } = null!;
}
