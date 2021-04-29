
namespace Core.DTOs.ServiceNow
{
    public class UpdateTicketResponseDto
    {
        public UpdateTicketResponse result { get; set; }
    }
    public class UpdateTicketResponse
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public UpdateTicketResponseDetails Response { get; set; }
    }
    public class UpdateTicketResponseDetails
    {
        public string Number { get; set; }
        public string Short_Description { get; set; }
        public string State { get; set; }
        public string State_Id { get; set; }
        public string Work_notes { get; set; }
    }
}
