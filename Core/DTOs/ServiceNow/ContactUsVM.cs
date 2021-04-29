namespace Core.DTOs.ServiceNow
{
    public class ContactUsVM
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public TicketVM TicketVM { get; set; }
    }
}
