using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{
    public interface IAuthDataService
    {
        Task<IResult<bool>> IsDupplicateEmailAsync(string email);
        Task<IResult<bool>> IsExistUserNameAsync(string userName);
        Task<IResult<TokenResponse>> GetTokenAsync(SignInByEmailRequest loginRequest);
        Task<IResult<TokenResponse>> RefreshToken(string jwtToken, string refreshToken);
        Task<IResult<string>> RegisterAsync(RegisterRequest registerRequest);
        Task<IResult<bool>> TwoFactorLoginByPhoneNumber(string phoneNumber);
        Task<IResult<bool>> TwoFactorRegisterByPhoneNumberAsync(string phoneNumber);
        Task<IResult<bool>> VerifyEmailAsync(string email, string token);
        Task<IResult<TokenResponse>> VerifyPhoneNumberAsync(string phoneNumber, string token);
    }
}