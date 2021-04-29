using System.Collections.Generic;

namespace Core.DTOs.ServiceNow
{
    public class ServiceNowLookUpResponseDto
    {
        public ServiceNowLookUpResponse result { get; set; }
    }
    public class ServiceNowLookUpResponse
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public IList<ServiceNowLookUpResponseDetails> Response { get; set; }
    }
    public class ServiceNowLookUpResponseDetails
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}
