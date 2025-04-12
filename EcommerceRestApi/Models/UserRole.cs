using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

[PrimaryKey("UserId", "RoleId")]
public partial class UserRole
{
    [Key]
    public string UserId { get; set; } = null!;

    [Key]
    public string RoleId { get; set; } = null!;
}
