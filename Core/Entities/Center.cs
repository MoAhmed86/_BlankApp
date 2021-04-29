using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Center : BaseEntity
    {
        [StringLength(50)]
        public string Name { get; set; }
        
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Desc { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(15)]
        public string PhoneNo { get; set; }
        /// <summary>
        /// السجل التجاري
        /// </summary>
        [StringLength(50)]
        public string CommercialRegister { get; set; }

        [StringLength(50)]
        public string Email { get; set; }
        
        /// <summary>
        /// حكومي | خاص
        /// </summary>
        public bool IsGovernmental { get; set; }

        /// <summary>
        /// خط الطول
        /// </summary>
        public decimal? Longitude { get; set; }
        /// <summary>
        /// خط العرض
        /// </summary>
        public decimal? Latitude { get; set; }
    }
}
