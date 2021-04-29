
namespace Core.DTOs.ServiceNow
{
    public class TicketDto
    {
        public string u_source { get; set; }
        public string category { get; set; }
        public string subcategory { get; set; }
        public string u_customer_name { get; set; }
        public string short_description { get; set; }
        public string u_customer_email { get; set; }
        public string u_id { get; set; }
        public string u_mobile { get; set; }
        public string description { get; set; }
        public string comments { get; set; }
        public string Number { get; set; }
        public string Sys_id { get; set; }
        //{"u_source":"RawaafedAdmin","category":"45","subcategory":"4700","u_customer_name":"Tetco customer","short_description":"Created incident through REST API","u_customer_email":"Tetco@t.com","u_id":"1234567890","u_mobile":"966123456789","description":"Testing Positive cycle for case creation through REST API","comments":"Comments added"}
    }
}
