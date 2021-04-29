using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public partial class LookupItem : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }

        [Required, MaxLength(50)]
        public string DescAr { get; set; }

        [MaxLength(50)]
        public string DescEn { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual LookupItem ParentLookupItem { get; set; }
    }
}
