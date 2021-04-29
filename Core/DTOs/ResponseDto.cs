using static Core.Enums;
using System.Net;
using System.Collections.Generic;

namespace Core.DTOs
{
    public class ResponseHttp<T> : ResponseHttp
    {
        public T Data { get; set; }
    }

    public class ResponseHttp
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public IList<ValidationErrorModel> ValidationErrors { get; set; } = new List<ValidationErrorModel>();
    }

    public class ResponseDto<T> : ResponseDto
    {
        public T Data { get; set; }
    }

    public class ResponseDto
    {
        public StatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public IList<ValidationErrorModel> ValidationErrors { get; set; } = new List<ValidationErrorModel>();
    }

    public class ValidationErrorModel
    {
        public string ErrorMessage { get; set; }
        public string PropertyName { get; set; }
    }
}
