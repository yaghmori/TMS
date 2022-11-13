using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{
    public interface ICultureDataService
    {
        Task<IResult<string>> AddCultureAsync(CultureRequest request);
        Task<IResult<bool>> DeleteCultureByIdAsync(string cultureId);
        Task<IResult<CultureResponse>> GetCultureByIdAsync(string cultureId);
        Task<IResult<List<CultureResponse>>> GetCulturesAsync();
        Task<IResult<bool>> UpdateCultureAsync(string cultureId, CultureRequest request);
    }
}