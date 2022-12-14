using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TMS.Shared.Helpers;
using TMS.Shared.Models;
using TMS.Shared.ResultWrapper;
using TMS.Web.Server.Extensions;

namespace TMS.API.Controllers
{
    [Authorize]
    [Route("api/v1/server/config")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        [HttpGet("JwtSettings")]
        public async Task<IActionResult> GetJwtSettings()
        {
            try
            {
                return Ok(await Result<JwtSettings>.SuccessAsync(ConfigHelper.JwtSettings));
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }
        [HttpGet("SmtpSettings")]
        public async Task<IActionResult> GetSmtpSettings()
        {
            try
            {
                return Ok(await Result<SmtpSettings>.SuccessAsync(ConfigHelper.SmtpSettings));
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        [HttpGet("TokenValidationParameters")]
        public async Task<IActionResult> GetValidationParameters()
        {
            try
            {
                return Ok(await Result<TokenValidationParameters>.SuccessAsync(ConfigHelper.ValidationParameters));
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        [HttpPut("JwtSettings")]
        public async Task<IActionResult> UpdateJwtSettings(JwtSettings settings)
        {
            try
            {
                ConfigHelper.SetJwtSettings(settings);
                return Ok(await Result.SuccessAsync("Setting successfully updated."));
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        [HttpPut("SmtpSettings")]
        public async Task<IActionResult> UpdateSmtpSettings(SmtpSettings settings)
        {
            try
            {
                ConfigHelper.SetSmtpSettings(settings);
                return Ok(await Result.SuccessAsync("Setting successfully updated."));
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


    }
}
