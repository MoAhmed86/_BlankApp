using Core.DTOs;
using System;
using System.Threading.Tasks;
using static Core.Enums;

namespace Core.Services
{
    public interface ICommonFunctions
    {
        string ConvertUmAlQuraToGregString(string umAlQuraDate);
        DateTime ConvertUmAlQuraToGregDate(string umAlQuraDate);
        bool SendEmail(string emailAddress, string body, string subject, EmailTypes emailType, string title = null);
        Task<bool> ReCaptchaPassed(string gRecaptchaResponse, string secret);
        void LogError(Exception ex, Object model);
        void LogError(Exception ex, string message = "");

        PagingModel PreparePagingModel(int pageIndex, int rowsCount);
    }
}
