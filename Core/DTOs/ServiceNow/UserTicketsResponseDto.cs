using System.Collections.Generic;

namespace Core.DTOs.ServiceNow
{
    public class UserTicketsResponseDto
    {
        public UserTicketsResponse result { get; set; }
    }
    public class UserTicketsResponse
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public IList<UserTicketsResponseDetails> Response { get; set; }
    }
    public class UserTicketsResponseDetails
    {
        public decimal Record_Number { get; set; }
        public string Number { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Short_Description { get; set; }
        public string Description { get; set; }
        public string Contact_Type { get; set; }
        public string State { get; set; }
        public string State_Id { get; set; }
        public string Priority { get; set; }
        public string Repeated_Case { get; set; }
        public string ID { get; set; }
        public string Customer_Email { get; set; }
        public string Customer_Name { get; set; }
        public string Mobile { get; set; }
        public string Created_On { get; set; }
        public string Created_By { get; set; }
        public string Last_Updated { get; set; }
        public string Last_Updated_By { get; set; }
        public string Last_Comment { get; set; }
        public string Resolution_Notes { get; set; }
        public string Work_Notes { get; set; }
        public string Sys_id { get; set; }
    }
}
