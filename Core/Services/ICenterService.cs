using Core.DTOs;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface ICenterService
    {
        Task<ResponseDto<CenterDto>> GetAsync(int centerId);
        Task<ResponseDto<int>> SaveAsync(CenterDto dto);
    }
}
