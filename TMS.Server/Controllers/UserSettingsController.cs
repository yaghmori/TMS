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
    [Route("api/v1/server/UserSettings")]
    [ApiController]

    public class UserSettingsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<ServerDbContext> _unitOfWork;

        public UserSettingsController(IMapper mapper, IUnitOfWork<ServerDbContext> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }




        /// <summary>
        /// Add new UserSetting 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>UserSettingId as string</returns>
        [HttpPost]
        [Authorize(Policy = ApplicationPermissions.UserSetting.Create)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> AddUserSetting(UserSettingsRequest request)
        {
            try
            {
                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(request.UserId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                var isDupplicate = await _unitOfWork.UserSettings.AnyAsync(x => x.UserId == user.Id);
                if (isDupplicate)
                    return Conflict(await Result.FailAsync("UserSetting already defiend."));

                var userSetting = _mapper.Map<UserSetting>(request);

                await _unitOfWork.UserSettings.AddAsync(userSetting);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result<string>.SuccessAsync(data: userSetting.Id.ToString()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Get all UserSettings
        /// </summary>
        /// <returns>List of UserSettingsResponse</returns>
        [HttpGet]
        [Authorize(Policy = ApplicationPermissions.UserSetting.View)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<UserSettingsResponse>))]
        public async Task<IActionResult> GetUserSettings(string? query = null)
        {
            try
            {
                var userSettings = await _unitOfWork.UserSettings.GetAllAsync(predicate: x => !string.IsNullOrWhiteSpace(query) ? x.UserId.ToString().Contains(query) : true,
                    selector: SelectExpressions.UserSettings.UserSettingsResponse);
                return Ok(await Result<IList<UserSettingsResponse>>.SuccessAsync(userSettings));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Get UserSetting by id
        /// </summary>
        /// <param name="userSettingsId"></param>
        /// <returns>UserSettingsResponse</returns>
        [HttpGet("{userSettingsId}")]
        [Authorize(Policy = ApplicationPermissions.UserSetting.View)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserSettingsResponse))]
        public async Task<IActionResult> GetUserSettingsById(string userSettingsId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userSettingsId))
                    return BadRequest(await Result.FailAsync("UserSettingsId is null or empty."));

                var userSetting = await _unitOfWork.UserSettings.GetFirstOrDefaultAsync(predicate: x => x.Id.ToString().Equals(userSettingsId),
                    selector: SelectExpressions.UserSettings.UserSettingsResponse);

                if (userSetting == null)
                    return NotFound(await Result.FailAsync("UserSetting not found."));

                return Ok(await Result<UserSettingsResponse>.SuccessAsync(userSetting));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get UserSetting by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>UserSettingsResponse</returns>
        [HttpGet("users/{userId}")]
        [Authorize(Policy = ApplicationPermissions.UserSetting.View)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserSettingsResponse))]
        public async Task<IActionResult> GetUserSettingsByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var userSetting = await _unitOfWork.UserSettings.GetFirstOrDefaultAsync(predicate: x => x.UserId.ToString().Equals(userId),
                    selector: SelectExpressions.UserSettings.UserSettingsResponse);

                if (userSetting == null)
                    return NotFound(await Result.FailAsync("UserSetting not found."));

                return Ok(await Result<UserSettingsResponse>.SuccessAsync(userSetting));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Delete UserSetting by id
        /// </summary>
        /// <param name="userSettingsId"></param>
        /// <returns></returns>
        [HttpDelete("{userSettingsId}")]
        [Authorize(Policy = ApplicationPermissions.UserSetting.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserSettingById(string userSettingsId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userSettingsId))
                    return BadRequest(await Result.FailAsync("UserSettingsId is null or empty."));

                var userSetting = await _unitOfWork.UserSettings.FindAsync(Guid.Parse(userSettingsId));
                if (userSetting is null)
                    return NotFound(await Result.FailAsync("UserSetting not found."));

                _unitOfWork.UserSettings.Remove(userSetting);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("UserSetting successfully deleted"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Update UserSetting
        /// </summary>
        /// <param name="userSettingsId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{userSettingsId}")]
        [Authorize(Policy = ApplicationPermissions.UserSetting.Edit)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserSettingById(string userSettingsId, [FromBody] JsonPatchDocument<UserSettingsRequest> request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userSettingsId))
                    return BadRequest(await Result.FailAsync("UserSettingsId is null or empty."));

                var userSetting = await _unitOfWork.UserSettings.FindAsync(Guid.Parse(userSettingsId));
                if (userSetting is null)
                    return NotFound(await Result.FailAsync("UserSetting not found."));

                var requestToPatch = _mapper.Map<UserSettingsRequest>(userSetting);
                request.ApplyTo(requestToPatch);
                _mapper.Map(requestToPatch, userSetting);

                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("UserSetting successfully updated"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }
    }
}
