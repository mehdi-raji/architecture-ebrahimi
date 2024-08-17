using System.ComponentModel.DataAnnotations;
using Entities.Common;

namespace Entities
{
    public class Role : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}
