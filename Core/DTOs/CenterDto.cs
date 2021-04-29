
namespace Core.DTOs
{
    public class CenterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string Title { get; set; }

        public string Desc { get; set; }

        public string City { get; set; }

        public string PhoneNo { get; set; }
        /// <summary>
        /// السجل التجاري
        /// </summary>
        public string CommercialRegister { get; set; }

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
