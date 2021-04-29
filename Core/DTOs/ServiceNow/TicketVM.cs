using System;

namespace Core.DTOs.ServiceNow
{
    public class TicketVM
    {
        public string Source { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ShortDescription { get; set; }
        public string IdentityNumber { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string EMail { get; set; }
        public string TicketNumber { get; set; }
        public string SystemId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateHijri { get; set; }
        public string LastUpdatedDate { get; set; }
        public string LastUpdatedDateHijri { get; set; }
        public string State { get; set; }
        public bool IsLoggedIn { get; set; }
        public byte[] PhotoFile { get; set; }
        public string RecaptchaStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int StateId { get; set; }
        public string StateColor { get; set; }
    }
}
