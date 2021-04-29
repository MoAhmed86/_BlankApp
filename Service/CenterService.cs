using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Services;
using DB;
using System.Threading.Tasks;

namespace Service
{
    public class CenterService : ServiceRepo<Center>, ICenterService
    {
        private readonly AppDBContext context;
        private readonly IMapper mapper;

        public CenterService(AppDBContext context, IMapper mapper) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ResponseDto<CenterDto>> GetAsync(int centerId)
        {
            ResponseDto<CenterDto> responseDto = new ResponseDto<CenterDto>();

            var entity = await GetByIdAsync(centerId);
            if (entity != null)
            {
                responseDto.Data = mapper.Map<CenterDto>(entity);
                responseDto.StatusCode = Core.Enums.StatusCode.OK;
            }
            else
                responseDto.StatusCode = Core.Enums.StatusCode.NotFound;

            return responseDto;
        }

        public async Task<ResponseDto<int>> SaveAsync(CenterDto dto)
        {
            ResponseDto<int> responseDto = new ResponseDto<int>();

            if (dto.Id > 0)
            {
                //Edit
                var entity = await GetByIdAsync(dto.Id);
                mapper.Map(dto, entity, typeof(CenterDto), typeof(Center));
                Update(entity);

                responseDto.StatusCode = Core.Enums.StatusCode.Updated;
            }
            else
            {
                //New
                var entity = mapper.Map<Center>(dto);
                AddAsync(entity);

                responseDto.StatusCode = Core.Enums.StatusCode.OK;
            }

            responseDto.Data = await context.SaveChangesAsync();
            if (responseDto.Data == 0)
                responseDto.StatusCode = Core.Enums.StatusCode.SaveFailed;

            return responseDto;
        }
    }
}
