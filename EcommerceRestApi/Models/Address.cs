using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Index("CountryId", Name = "IX_Addresses_CountryId")]
[Index("CustomerId", Name = "IX_Addresses_CustomerId")]
public partial class Address : EntityBase
{
    public int CustomerId { get; set; }

    public int? CountryId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Street { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? HouseNumber { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? FlatNumber { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? City { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? State { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? PostalCode { get; set; }


    [ForeignKey("CountryId")]
    [InverseProperty("Addresses")]
    public virtual Country? Country { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Addresses")]
    public virtual Customer Customer { get; set; } = null!;
}
