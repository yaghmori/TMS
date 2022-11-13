using AutoMapper;
using Infrastructure.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Security.Claims;
using TMS.Core.Domain.Entities;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.Constants;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;
using TMS.Web.Server.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.API.Controllers
{
    [Authorize]
    [Route("api/v1/server/AppSettings")]
    [ApiController]

    public class AppSettingsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<ServerDbContext> _unitOfWork;

        public AppSettingsController(IMapper mapper, IUnitOfWork<ServerDbContext> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }




        /// <summary>
        /// Add new AppSetting 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>AppSettingId as string</returns>
        [HttpPost]
        [Authorize(Policy = ApplicationPermissions.AppSetting.Create)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> AddAppSetting(AppSettingsRequest request)
        {
            try
            {
                if (await _unitOfWork.AppSettings.AnyAsync(x => x.Key.Equals(request.Key)))
                    return Conflict(await Result.FailAsync("key is already defined."));

                var appSetting = _mapper.Map<AppSetting>(request);
                await _unitOfWork.AppSettings.AddAsync(appSetting);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result<string>.SuccessAsync(data: appSetting.Id.ToString()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get all AppSettings
        /// </summary>
        /// <returns>List of AppSettingsResponse</returns>
        [HttpGet]
        [Authorize(Policy = ApplicationPermissions.AppSetting.View)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<AppSettingsResponse>))]
        public async Task<IActionResult> GetAppSettings(string? query = null)
        {
            try
            {
                var appSettings = await _unitOfWork.AppSettings.GetAllAsync(predicate: x => (!string.IsNullOrWhiteSpace(query) ? x.Key.Contains(query) : true),
                    selector: SelectExpressions.AppSettings.AppSettingsResponse);
                return Ok(await Result<IList<AppSettingsResponse>>.SuccessAsync(appSettings));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get AppSetting by id
        /// </summary>
        /// <param name="appSettingsId"></param>
        /// <returns>AppSettingsResponse</returns>
        [HttpGet("{appSettingsId}")]
        [Authorize(Policy = ApplicationPermissions.AppSetting.View)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppSettingsResponse))]
        public async Task<IActionResult> GetAppSettingsById(string appSettingsId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(appSettingsId))
                    return BadRequest(await Result.FailAsync("AppSettingsId is null or empty."));

                var appSetting = await _unitOfWork.AppSettings.GetFirstOrDefaultAsync(predicate: x => x.Id.ToString().Equals(appSettingsId),
                    selector: SelectExpressions.AppSettings.AppSettingsResponse);

                if (appSetting == null)
                    return NotFound(await Result.FailAsync("AppSetting not found."));

                return Ok(await Result<AppSettingsResponse>.SuccessAsync(appSetting));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Get AppSetting by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>AppSettingsResponse</returns>
        [HttpGet("keys")]
        [Authorize(Policy = ApplicationPermissions.AppSetting.View)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AppSettingsResponse))]
        public async Task<IActionResult> GetAppSettingsByKey(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(await Result.FailAsync("Key is null or empty."));

                var appSetting = await _unitOfWork.AppSettings.GetFirstOrDefaultAsync(predicate: x => x.Key.Equals(key),
                    selector: SelectExpressions.AppSettings.AppSettingsResponse);

                if (appSetting == null)
                    return NotFound(await Result.FailAsync("AppSetting not found."));

                return Ok(await Result<AppSettingsResponse>.SuccessAsync(appSetting));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Delete AppSetting by id
        /// </summary>
        /// <param name="appSettingsId"></param>
        /// <returns></returns>
        [HttpDelete("{appSettingsId}")]
        [Authorize(Policy = ApplicationPermissions.AppSetting.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAppSettingById(string appSettingsId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(appSettingsId))
                    return BadRequest(await Result.FailAsync("AppSettingsId is null or empty."));

                var appSetting = await _unitOfWork.AppSettings.FindAsync(Guid.Parse(appSettingsId));
                if (appSetting is null)
                    return NotFound(await Result.FailAsync("AppSetting not found."));

                _unitOfWork.AppSettings.Remove(appSetting);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("AppSetting successfully deleted"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Delete AppSetting by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Policy = ApplicationPermissions.AppSetting.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAppSettingByKey(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return BadRequest(await Result.FailAsync("Key is null or empty."));

                var appSetting = await _unitOfWork.AppSettings.GetFirstOrDefaultAsync(predicate: x => x.Key.Equals(key));
                if (appSetting is null)
                    return NotFound(await Result.FailAsync("AppSetting not found."));

                _unitOfWork.AppSettings.Remove(appSetting);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("AppSetting successfully deleted"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Update AppSetting
        /// </summary>
        /// <param name="appSettingId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{appSettingId}")]
        [Authorize(Policy = ApplicationPermissions.AppSetting.Edit)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAppSettingById(string appSettingId, [FromBody] JsonPatchDocument<AppSettingsRequest> request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(appSettingId))
                    return BadRequest(await Result.FailAsync("AppSettingId is null or empty."));

                var appSetting = await _unitOfWork.AppSettings.FindAsync(Guid.Parse(appSettingId));
                if (appSetting is null)
                    return NotFound(await Result.FailAsync("AppSetting not found."));

                var requestToPatch = _mapper.Map<AppSettingsRequest>(appSetting);
                request.ApplyTo(requestToPatch);
                _mapper.Map(requestToPatch, appSetting);

                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("AppSetting successfully updated"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }
    }
}
