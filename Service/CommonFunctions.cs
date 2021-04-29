using Core.DTOs;
using Core.Resources;
using Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using static Core.Enums;

namespace Service
{
    public class CommonFunctions : ICommonFunctions
    {
        private readonly IOptions<Configuration> config;
        private readonly IHostingEnvironment host;

        public CommonFunctions(IOptions<Configuration> config, IHostingEnvironment host)
        {
            this.config = config;
            this.host = host;
        }
        public string ConvertUmAlQuraToGregString(string umAlQuraDate)
        {
            return ConvertUmAlQuraToGregDate(umAlQuraDate).ToString("dd/MM/yyyy");
        }

        public DateTime ConvertUmAlQuraToGregDate(string umAlQuraDate)
        {
            var hijri = new UmAlQuraCalendar();

            int year = int.Parse(umAlQuraDate.Split('/')[2]);
            int month = int.Parse(umAlQuraDate.Split('/')[1]);
            int day = int.Parse(umAlQuraDate.Split('/')[0]);

            return new DateTime(year, month, day, hijri);
        }

        public bool SendEmail(string emailAddress, string body, string subject, EmailTypes emailType, string title = null)
        {
            MailMessage mail = new MailMessage(config.Value.SMTPFromAddress, emailAddress);
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(config.Value.SMTPUser, config.Value.SMTPUserPwd, config.Value.SMTPDomain);

            client.Host = config.Value.SMTPServer;
            client.Port = int.Parse(config.Value.SMTPPort);
            client.EnableSsl = Convert.ToBoolean(config.Value.SMTPUseSSL);
            mail.Subject = subject;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Body = GetEmailBody(title, body, emailType);
            try
            {
                client.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex, new { emailAddress, body, subject, emailType });
                return false;
            }
        }

        private string GetEmailBody(string emailTitle, string emailBody, EmailTypes emailType)
        {
            string body = string.Empty;
            if (emailType == EmailTypes.NewPassword)
                body = GetHTMLContent(emailTitle, emailBody, "ResetPassword");
            if (emailType == EmailTypes.NewRegistration)
                body = GetHTMLContent(emailTitle, emailBody, "NewRegistration");
            else if (emailType == EmailTypes.ProjectDelivered)
            {
                body = GetHTMLContent(emailTitle, emailBody.Split("*")[0], "ProjectDelivered").Replace("{ContentURL", emailBody.Split("*")[1]);
            }
            else if (emailType == EmailTypes.Generic)
            {
                //switch (platform)
                //{
                //    case (int)Platforms.Thraa:
                //        body = GetHTMLContent(emailTitle, emailBody, "Thraa/Generic");
                //        break;
                //    case (int)Platforms.Rawafid:
                //        body = GetHTMLContent(emailTitle, emailBody, "Rawaafid/Generic");
                //        break;
                //    default:
                //        body = GetHTMLContent(emailTitle, emailBody, "Hulool/Generic");
                //        break;
                //}
            }
            //else if (emailType == EmailTypes.SendToFriend)
            //    body = GetHTMLContent(emailBody, "ResetPassword");
            //else if(emailType == EmailTypes.ProjectDelivered)
            //    body = GetHTMLContent(emailBody, "ResetPassword");
            return body;
        }
        private string GetHTMLContent(string emailTitle, string emailBody, string htmlFileName)
        {
            string body;
            using (StreamReader reader = new StreamReader($"{ host.WebRootPath}/html/{htmlFileName}.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Content}", emailBody);
            if (!string.IsNullOrEmpty(emailTitle))
                body = body.Replace("{Title}", emailTitle);
            return body;
        }
        //private string GetContactUsEmailBody(string emailBody)
        //{
        //    string body;
        //    using (StreamReader reader = new StreamReader($"{ host.WebRootPath}/html/EmailContactUs.html"))
        //    {
        //        body = reader.ReadToEnd();
        //    }
        //    body = body.Replace("{messageBody}", emailBody);
        //    return body;
        //}
        //private string GetSentToFriendBody(string emailBody)
        //{
        //    string body;
        //    using (StreamReader reader = new StreamReader($"{ host.WebRootPath}/html/SendtoFriend.html"))
        //    {
        //        body = reader.ReadToEnd();
        //    }
        //    body = body.Replace("{messageBody}", emailBody);
        //    return body;
        //}
        private AlternateView CreateHtmlMessage(string message, string logoPath)
        {
            var inline = new LinkedResource(logoPath, MediaTypeNames.Image.Jpeg);
            inline.ContentId = "Email";

            var alternateView = AlternateView.CreateAlternateViewFromString(
                                    message,
                                    Encoding.UTF8,
                                    MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(inline);

            return alternateView;
        }

        public PagingModel PreparePagingModel(int pageIndex, int rowsCount)
        {
            PagingModel model = new PagingModel();
            model.CurrentPageInex = pageIndex;
            model.ItemsCount = rowsCount;
            model.PagesCount = (model.ItemsCount / config.Value.PageSize) - (model.ItemsCount % config.Value.PageSize == 0 ? 1 : 0);

            return model;
        }

        #region SeriLog

        public void LogError(string message)
        {
            Log.Error(message);
        }

        public void LogError(Exception ex, string message = "")
        {
            //string logMessage = $"Message : {message} {Environment.NewLine} /*******/ {Environment.NewLine} Exception : {Newtonsoft.Json.JsonConvert.SerializeObject(ex)}";
            //log(logMessage);

            Log.Error(ex, message);
        }

        public void LogError(Exception ex, Object model)
        {
            //string logMessage = $"Model : {Newtonsoft.Json.JsonConvert.SerializeObject(model)} /********/ {Environment.NewLine} Eception : {Environment.NewLine} {Newtonsoft.Json.JsonConvert.SerializeObject(ex)}";
            //log(logMessage);

            Log.Error(ex, Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }

        //private void log(string logMessage)
        //{
        //    string contentRootPath = host.ContentRootPath;
        //    string errorsFilePath = $"{contentRootPath}/Logs/ErrorLog_{DateTime.Now.ToString("dd-MM-yyyy")}.txt";
        //    StringBuilder logData = new StringBuilder();

        //    if (!File.Exists(errorsFilePath))
        //        File.CreateText(errorsFilePath).Close();
        //    else
        //        logData.Append(File.ReadAllText(errorsFilePath));

        //    logData.Append(logMessage);
        //    logData.Append($"{Environment.NewLine} **************************************************** {Environment.NewLine}");

        //    File.WriteAllText(errorsFilePath, logData.ToString());
        //}


        #endregion

        #region Google ReCaptcha

        public async Task<bool> ReCaptchaPassed(string gRecaptchaResponse, string secret)
        //, ILogger logger)
        {
            if (!config.Value.ValidateReCaptcha)
                return true;

            HttpClient httpClient = new HttpClient();
            var res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={gRecaptchaResponse}").Result;
            if (res.StatusCode != HttpStatusCode.OK)
            {
                //logger.LogError("Error while sending request to ReCaptcha");
                return false;
            }

            string JSONres = await res.Content.ReadAsStringAsync();
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
