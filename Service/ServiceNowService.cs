using AutoMapper;
using Core.DTOs;
using Core.DTOs.Identity;
using Core.DTOs.ServiceNow;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Core.Enums;

namespace Service
{
    public class ServiceNowService : IServiceNowService
    {
        private readonly IMapper mapper;
        private readonly IOptions<Configuration> config;
        private readonly IMimeMappingService mimeMappingService;
        private readonly UserManager<ApplicationUser> userManager;

        public ServiceNowService(IMapper mapper, IOptions<Configuration> config, IMimeMappingService mimeMappingService, UserManager<ApplicationUser> userManager)
        {
            this.mapper = mapper;
            this.config = config;
            this.mimeMappingService = mimeMappingService;
            this.userManager = userManager;
        }

        public async Task<ResponseDto<string>> CreateTicket(TicketVM model, IFormFileCollection attachments, ApplicationUser applicationUser)
        {
            var ticketDto = mapper.Map<TicketDto>(model);

            if (applicationUser != null)
            {
                ticketDto.u_customer_email = applicationUser.Email;
                ticketDto.u_customer_name = applicationUser.FullName;
                ticketDto.u_id = applicationUser.IdentityNumber;
                ticketDto.u_mobile = "+966" + applicationUser.PhoneNumber.Substring(1);

            }

            HttpClient client = CreateHttpClient();
            var stringPayload = JsonConvert.SerializeObject(ticketDto);
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var responseTask = await client.PostAsync($"/api/sn_customerservice/csmtetcoapi/sn_customerservice_case", httpContent);

            if (responseTask.IsSuccessStatusCode)
            {

                var readTask = await responseTask.Content.ReadAsStringAsync();

                var serviceNowResult = JsonConvert.DeserializeObject<InsertTicketResponseDto>(readTask);

                if (serviceNowResult.result.Status.ToLower() == "failure")
                    return new ResponseDto<string> { StatusCode = StatusCode.SaveFailed };

                var photoFile = attachments.Count > 0 ? attachments[0] : null;
                if (photoFile != null)
                {
                    byte[] imageByte = null;
                    BinaryReader rdr = new BinaryReader(photoFile.OpenReadStream());
                    imageByte = rdr.ReadBytes((int)photoFile.Length);
                    model.PhotoFile = imageByte;
                    var fileUploaded = UploadTicketAttachment(photoFile.FileName, model.PhotoFile, serviceNowResult.result.Response.sys_id);
                    if (!fileUploaded)
                        return new ResponseDto<string> { StatusCode = StatusCode.SaveFailed };
                }
                return new ResponseDto<string> { StatusCode = StatusCode.OK, Data = serviceNowResult.result.Response.Number };
            }
            else
            {
                return new ResponseDto<string> { StatusCode = StatusCode.SaveFailed };
            }
        }
        private bool UploadTicketAttachment(string fileName, byte[] attachment, string sysId)
        {
            var _client = new RestClient(new Uri(config.Value.ServiceNowBaseUrl));
            _client.Authenticator = new HttpBasicAuthenticator(config.Value.ServiceNowUserName, config.Value.ServiceNowPassword);


            var restRequest = new RestRequest("/api/now/attachment/upload", Method.POST)
            {
                AlwaysMultipartFormData = true
            };

            restRequest.AddParameter("table_name", "sn_customerservice_case");
            restRequest.AddParameter("table_sys_id", sysId);
            restRequest.AddParameter("file", string.Empty);
            restRequest.AddFile("file_name", attachment, fileName, mimeMappingService.Map(fileName));

            var response = _client.Execute(restRequest);

            if (response.IsSuccessful)
            {
                return true;
            }
            else
                return false;

        }
        public async Task<ResponseDto<IList<ServiceNowLookUpResponseDetails>>> GetCategories(string source)
        {
            HttpClient client = CreateHttpClient();
            var responseTask = client.GetAsync($"/api/sn_customerservice/csmtetcoapi/category/{source}");

            await responseTask;
            var response = responseTask.Result;
            if (response.IsSuccessStatusCode)
            {
                var readTask = response.Content.ReadAsStringAsync();
                readTask.Wait();

                var serviceNowResult = JsonConvert.DeserializeObject<ServiceNowLookUpResponseDto>(readTask.Result);
                return new ResponseDto<IList<ServiceNowLookUpResponseDetails>> { StatusCode = StatusCode.Found, Data = serviceNowResult.result.Response };
            }
            return new ResponseDto<IList<ServiceNowLookUpResponseDetails>> { StatusCode = StatusCode.NotFound };
        }
        public async Task<ResponseDto<IList<ServiceNowLookUpResponseDetails>>> GetSubCategories(int categoryId)
        {
            HttpClient client = CreateHttpClient();
            var responseTask = client.GetAsync($"/api/sn_customerservice/csmtetcoapi/subcategory/{config.Value.ServiceNowSourceName}/{categoryId}");
            await responseTask;
            var response = responseTask.Result;
            if (response.IsSuccessStatusCode)
            {
                var readTask = response.Content.ReadAsStringAsync();
                readTask.Wait();
                var serviceNowResult = JsonConvert.DeserializeObject<ServiceNowLookUpResponseDto>(readTask.Result);
                return new ResponseDto<IList<ServiceNowLookUpResponseDetails>> { StatusCode = StatusCode.Found, Data = serviceNowResult.result.Response };

            }
            return new ResponseDto<IList<ServiceNowLookUpResponseDetails>> { StatusCode = StatusCode.NotFound };
        }
        public async Task<ResponseDto<TicketRetrievalResponseDetails>> GetTicket(string ticketNumber)
        {
            HttpClient client = CreateHttpClient();
            var responseTask = client.GetAsync($"/api/sn_customerservice/csmtetcoapi/{config.Value.ServiceNowSourceName}/case_id/{ticketNumber}");

            await responseTask;
            var response = responseTask.Result;
            if (response.IsSuccessStatusCode)
            {
                var readTask = response.Content.ReadAsStringAsync();
                readTask.Wait();
                var serviceNowResult = JsonConvert.DeserializeObject<TicketRetrievalResponseDto>(readTask.Result);

                if (serviceNowResult.result.Status.ToLower() == "failure")
                    return new ResponseDto<TicketRetrievalResponseDetails> { StatusCode = StatusCode.NotFound };

                return new ResponseDto<TicketRetrievalResponseDetails> { StatusCode = StatusCode.Found, Data = serviceNowResult.result.Response };
            }
            return new ResponseDto<TicketRetrievalResponseDetails> { StatusCode = StatusCode.NotFound };

        }
        public async Task<ResponseDto<IList<UserTicketsResponseDetails>>> GetUserTickets(string source, string identityNumber)
        {
            HttpClient client = CreateHttpClient();
            var responseTask = client.GetAsync($"/api/sn_customerservice/csmtetcoapi/{source}/{identityNumber}");

            await responseTask;
            var response = responseTask.Result;
            if (response.IsSuccessStatusCode)
            {
                var readTask = response.Content.ReadAsStringAsync();
                readTask.Wait();
                var serviceNowResult = JsonConvert.DeserializeObject<UserTicketsResponseDto>(readTask.Result);

                if (serviceNowResult.result.Status.ToLower() == "failure")
                    return new ResponseDto<IList<UserTicketsResponseDetails>> { StatusCode = StatusCode.NotFound };

                return new ResponseDto<IList<UserTicketsResponseDetails>> { StatusCode = StatusCode.Found, Data = serviceNowResult.result.Response };
            }
            return new ResponseDto<IList<UserTicketsResponseDetails>> { StatusCode = StatusCode.NotFound };

        }
        public async Task<ResponseDto<string>> AddCommentToTicket(string sysId, string comment)
        {
            HttpClient client = CreateHttpClient();
            var stringPayload = JsonConvert.SerializeObject(new { comments = comment });
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var responseTask = client.PostAsync($"/api/sn_customerservice/csmtetcoapi/sn_customerservice_case/{sysId}", httpContent);

            await responseTask;
            var response = responseTask.Result;
            if (response.IsSuccessStatusCode)
            {

                var readTask = response.Content.ReadAsStringAsync();
                readTask.Wait();

                var serviceNowResult = JsonConvert.DeserializeObject<UpdateTicketResponseDto>(readTask.Result);
                return new ResponseDto<string> { StatusCode = StatusCode.Updated, Data = serviceNowResult.result.Response.Number };

            }
            else
            {
                return new ResponseDto<string> { StatusCode = StatusCode.SaveFailed };
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(config.Value.ServiceNowBaseUrl);
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.Value.ServiceNowUserName}:{config.Value.ServiceNowPassword}")));
            return client;
        }
    }
}
