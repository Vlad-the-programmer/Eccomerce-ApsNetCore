using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceRestApi.Models
{
    public class EntityBase
    {
        public bool IsActive { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateCreated { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateUpdated { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateDeleted { get; set; }
    }
}
