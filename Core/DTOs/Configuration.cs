using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs
{
    public class Configuration
    {
        public string NourApi_Url { get; set; }
        public string NourApi_UserName { get; set; }
        public string NourApi_Password { get; set; }
        public string NourApi_Name { get; set; }

        public string SMTPFromAddress { get; set; }
        public string SMTPUser { get; set; }
        public string SMTPUserPwd { get; set; }
        public string SMTPDomain { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPPort { get; set; }
        public string SMTPUseSSL { get; set; }

        public bool ValidateReCaptcha { get; set; }
        public string GoogleReCaptchaSiteKey { get; set; }
        public string GoogleReCaptchaSecretKey { get; set; }

        public string ServiceNowSourceName { get; set; }
        public string ServiceNowBaseUrl { get; set; }
        public string ServiceNowUserName { get; set; }
        public string ServiceNowPassword { get; set; }

        public string RawafidApiUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string ThraaApiUrl { get; set; }
        public string ThraaApiUserName { get; set; }
        public string ThraaApiPassword { get; set; }

        public string PublishFolder { get; set; }

        public int PageSize { get; set; }

    }
}
