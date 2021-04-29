using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.DTOs;
using Core.DTOs.Api;
using Core.Repositories;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Core.Enums;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DefaultController : ControllerBase
    {
        private readonly IParticipationService participationService;
        private readonly ICommonFunctions commonFunctions;
        private readonly ICompetitionService competitionService;
        private readonly IFAQRepository fAQRepository;
        private readonly IMapper mapper;
        private readonly ITermRepository termRepository;

        public DefaultController(IParticipationService participationService, ICommonFunctions commonFunctions,
            ICompetitionService competitionService, IFAQRepository fAQRepository, IMapper mapper,
            ITermRepository termRepository)
        {
            this.participationService = participationService;
            this.commonFunctions = commonFunctions;
            this.competitionService = competitionService;
            this.fAQRepository = fAQRepository;
            this.mapper = mapper;
            this.termRepository = termRepository;
        }

        [HttpPost]
        [Route("Participation")]
        public async Task<ResponseHttp> CreateParticipation(ParticipationApiDto dto)
        {
            ResponseHttp response = new ResponseHttp();

            try
            {
                response = await participationService.Save(dto);
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, dto);
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return response;
        }

        [HttpGet]
        [Route("Participation/{Id}/{platformId}")]
        public async Task<ResponseHttp<ParticipationPreviewApiDto>> GetParticipationPreview(int id, int platformId = (int)Platforms.Hulool)
        {
            ResponseHttp<ParticipationPreviewApiDto> response = new ResponseHttp<ParticipationPreviewApiDto>();

            try
            {
                var res = await participationService.GetParticipationPreviewAsync(id,platformId);
                if (res.StatusCode == Core.Enums.StatusCode.OK)
                {
                    response.Data = res.Data;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex);
            }

            return response;
        }

        [HttpGet]
        [Route("Competitions/{platformId}")]
        public ResponseHttp<IList<CompetitionDto>> GetCompetitions(int platformId = (int)Platforms.Hulool)
        {
            ResponseHttp<IList<CompetitionDto>> response = new ResponseHttp<IList<CompetitionDto>>();

            try
            {
                var compResponse = competitionService.ListAsync_Api(platformId);
                if (compResponse.StatusCode == Core.Enums.StatusCode.OK)
                {
                    response.Data = compResponse.Data;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex);
            }

            return response;
        }

        [HttpGet]
        [Route("Competitions/{id}/{platformId}")]
        public async Task<ResponseHttp<CompetitionDto>> GetCompetitions(int id, int platformId = (int)Platforms.Hulool)
        {
            ResponseHttp<CompetitionDto> response = new ResponseHttp<CompetitionDto>();

            try
            {
                var compResponse = await competitionService.GetAsync(id, platformId);
                if (compResponse.StatusCode == Core.Enums.StatusCode.OK)
                {
                    response.Data = compResponse.Data;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex);
            }

            return response;
        }

        [HttpGet]
        [Route("CompetitionField/{id}/{platformId}")]
        public async Task<ResponseHttp<CompetitionFieldDto>> CompetitionField(int id, int platformId = (int)Platforms.Hulool)
        {
            ResponseHttp<CompetitionFieldDto> response = new ResponseHttp<CompetitionFieldDto>();

            try
            {
                var compResponse = await competitionService.GetCompetitionFieldAsync(id, platformId);
                if (compResponse.StatusCode == Core.Enums.StatusCode.OK)
                {
                    response.Data = compResponse.Data;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex);
            }

            return response;
        }

        [HttpGet]
        [Route("FAQs/{compId}/{platformId}")]
        public async Task<IActionResult> GeFAQs(int compId, int platformId = (int)Platforms.Hulool)
        {
            ResponseHttp<IList<FAQDto>> response = new ResponseHttp<IList<FAQDto>>();

            try
            {
                var faqList = await fAQRepository.GetByCompetitionIdAsync(compId, platformId);
                if (faqList != null && faqList.Count > 0)
                {
                    response.Data = mapper.Map<IList<FAQDto>>(faqList);
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("Terms/{compId}/{platformId}")]
        public async Task<IActionResult> GetTerms(int compId, int platformId = (int)Platforms.Hulool)
        {
            ResponseHttp<IList<TermDto>> response = new ResponseHttp<IList<TermDto>>();

            try
            {
                var terms = await termRepository.GetParentTermsAsync(compId, platformId);
                if (terms == null)
                    return NotFound();
                else
                {
                    response.Data = mapper.Map<IList<TermDto>>(terms);
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex);
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("Reply")]
        public async Task<IActionResult> AddReply(ParticipationLogApiDto dto)
        {
            try
            {
                ResponseDto res = await participationService.AddParticipationLogByProjectIdAsync(dto, (int)ParticipationStatus.Reply_Comment);
                if (res.StatusCode == Core.Enums.StatusCode.NotFound)
                    return NotFound();
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, dto);
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpGet]
        [Route("Replies/{projectId}/{pageIndex}/{platformId}")]
        public async Task<IActionResult> GetReplies(int projectId, int pageIndex, int platformId = (int)Platforms.Hulool)
        {
            ResponseHttp<PagedListModel<string>> response = new ResponseHttp<PagedListModel<string>>();
            try
            {
                ResponseDto<PagedListModel<string>> res = await participationService.GetParticipationLogsByProjectIdAsync(projectId, platformId, pageIndex, (int)ParticipationStatus.Reply_Comment);

                if (res.StatusCode == Core.Enums.StatusCode.NotFound)
                    return NotFound();
                else
                {
                    response.Data = res.Data;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                commonFunctions.LogError(ex, new { projectId, pageIndex, platformId });
                return StatusCode(500);
            }
        }
    }
}
