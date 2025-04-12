using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

[PrimaryKey("UserId", "LoginProvider", "ProviderKey")]
public partial class UserLogin
{
    [Key]
    public string LoginProvider { get; set; } = null!;

    [Key]
    public string ProviderKey { get; set; } = null!;

    [Key]
    public string UserId { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }
}
