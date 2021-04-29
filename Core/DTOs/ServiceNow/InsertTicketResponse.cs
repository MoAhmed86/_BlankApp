
namespace Core.DTOs.ServiceNow
{
    public class InsertTicketResponseDto
    {
        public InsertTicketResponse result { get; set; }
    }
    public class InsertTicketResponse
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public InsertTicketResponseDetails Response { get; set; }
    }
    public class InsertTicketResponseDetails
    {
        public string Number { get; set; }
        public string sys_id { get; set; }
    }
}
