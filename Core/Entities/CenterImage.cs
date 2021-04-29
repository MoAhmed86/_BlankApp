using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class CenterImage : BaseEntity
    {
        public int CenterId { get; set; }
        
        [MaxLength]
        public byte[] Image { get; set; }

        [ForeignKey("CenterId")]
        public virtual Center Center { get; set; }
    }
}
