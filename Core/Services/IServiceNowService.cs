using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTOs;
using Core.DTOs.Identity;
using Core.DTOs.ServiceNow;
using Microsoft.AspNetCore.Http;

namespace Core.Services
{
    public interface IServiceNowService
    {
        Task<ResponseDto<string>> AddCommentToTicket(string sysId, string comment);
        Task<ResponseDto<string>> CreateTicket(TicketVM model, IFormFileCollection attachments, ApplicationUser applicationUser);
        Task<ResponseDto<IList<ServiceNowLookUpResponseDetails>>> GetCategories(string source);
        Task<ResponseDto<IList<ServiceNowLookUpResponseDetails>>> GetSubCategories(int categoryId);
        Task<ResponseDto<TicketRetrievalResponseDetails>> GetTicket(string ticketNumber);
        Task<ResponseDto<IList<UserTicketsResponseDetails>>> GetUserTickets(string source, string identityNumber);
    }
}