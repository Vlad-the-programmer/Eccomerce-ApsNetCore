using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

[Index("CountryCode", Name = "UQ__Countrie__5D9B0D2C9915823B", IsUnique = true)]
[Index("CountryName", Name = "UQ__Countrie__E056F201CC4E645C", IsUnique = true)]
public partial class Country : EntityBase
{

    [StringLength(50)]
    [Unicode(false)]
    public string CountryName { get; set; } = null!;

    [StringLength(5)]
    [Unicode(false)]
    public string CountryCode { get; set; } = null!;

    [InverseProperty("Country")]
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
}
