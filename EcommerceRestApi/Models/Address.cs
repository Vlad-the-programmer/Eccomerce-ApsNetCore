using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

public partial class Address : EntityBase
{
    public int CustomerId { get; set; }

    public int? CountryId { get; set; }

    [StringLength(100, ErrorMessage = "Street should be no more than 100 characters long.")]
    [Unicode(false)]
    public string? Street { get; set; }

    [StringLength(20, ErrorMessage = "Housenumber should be no more than 20 characters long.")]
    [Unicode(false)]
    public string? HouseNumber { get; set; }

    [StringLength(20, ErrorMessage = "Flatnumber should be no more than 20 characters long.")]
    [Unicode(false)]
    public string? FlatNumber { get; set; }

    [StringLength(50, ErrorMessage = "City should be no more than 50 characters long.")]
    [Unicode(false)]
    public string? City { get; set; }

    [StringLength(50, ErrorMessage = "State should be no more than 50 characters long.")]
    [Unicode(false)]
    public string? State { get; set; }

    [StringLength(10, ErrorMessage = "Postalcode should be no more than 10 characters long.")]
    [Unicode(false)]
    public string? PostalCode { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("Addresses")]
    public virtual Country? Country { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Addresses")]
    public virtual Customer Customer { get; set; } = null!;
}
