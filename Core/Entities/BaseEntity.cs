using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public partial class BaseEntity
    {
        [Key]
        public virtual int Id { get; set; }
        public bool? IsDeleted { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public virtual int CreatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdatedBy { get; set; }
    }
}
