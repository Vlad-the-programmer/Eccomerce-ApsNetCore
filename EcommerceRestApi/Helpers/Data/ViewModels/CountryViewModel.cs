using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class CountryViewModel: BaseViewModel
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
