using System;
namespace Core.DTOs.ServiceNow
{
    public class UserTicketVM
    {
        public string Source { get; set; }
        public string IdentityNumber { get; set; }
        public string TicketNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string OrderType { get; set; }
    }
}
