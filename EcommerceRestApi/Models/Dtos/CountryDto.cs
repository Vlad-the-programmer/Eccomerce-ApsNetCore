using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Models.Dtos
{
    public class CountryDTO
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string CountryName { get; set; } = default!;

        [StringLength(5)]
        [Unicode(false)]
        public string CountryCode { get; set; } = default!;
    }
}
