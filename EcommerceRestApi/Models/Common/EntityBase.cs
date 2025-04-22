using EcommerceRestApi.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models
{
    public class EntityBase : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateCreated { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateUpdated { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateDeleted { get; set; }
    }
}
