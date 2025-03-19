using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceRestApi.Models.Dtos
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
    }
}
