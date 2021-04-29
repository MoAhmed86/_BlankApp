using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public partial class DataLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Transaction { get; set; }

        [Required]
        [StringLength(50)]
        public string TableName { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int RowId { get; set; }
    }
}
