using System;
using System.Collections.Generic;
using System.Text;
using static Core.Enums;

namespace Core.DTOs
{
    public class ResponseModel : ResponseDto
    {
        //public StatusCode StatusCode { get; set; }

        public IList<ValidationErrorModel> ValidationErrors { get; set; } = new List<ValidationErrorModel>();
        //public string Message { get; set; }
        //public string ImageMessage { get; set; }
    }

    public class ResponseModel<T> : ResponseModel
    {
        public T Data { get; set; }
    }

    public class ValidationErrorModel
    {
        public string ErrorMessage { get; set; }
        public string PropertyName { get; set; }
    }
}
