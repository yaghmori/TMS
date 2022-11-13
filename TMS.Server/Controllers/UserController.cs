using AutoMapper;
using Infrastructure.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using TMS.Core.Domain.Entities;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.Constants;
using TMS.Shared.PagedCollections;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;
using TMS.Web.Server.Extensions;
using TMS.Web.Server.Services;

namespace TMS.API.Controllers
{
    [Authorize]
    [Route("api/v1/server/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork<ServerDbContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ISmtpService _smtpService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenServices;
        private readonly bool _registerConfirmationRequired;

        public UserController(IUnitOfWork<ServerDbContext> unitOfWork, UserManager<User> userManager, IMapper mapper, IConfiguration configuration, ISmtpService messageSender, ITokenService tokenService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _smtpService = messageSender;
            _tokenService = tokenService;
            _configuration = configuration;
            _registerConfirmationRequired = Convert.ToBoolean(_configuration.GetSection("RegisterConfirmationRequired").Value);
            _unitOfWork = unitOfWork;
        }



        /// <summary>
        /// Add New User by email and without password. return newly added userId as string
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string</returns>
        [HttpPost("New")]
        [Authorize(Policy = ApplicationPermissions.User.Create)]
        public async Task<IActionResult> AddNewUser(UserRequest request)
        {

            try
            {
                if (await _unitOfWork.Users.AnyAsync(a => a.Email.Equals(request.Email)))
                    return Conflict(await Result.FailAsync("Email is is already exist."));

                var user = _mapper.Map<User>(request);
                user.ConcurrencyStamp = Guid.NewGuid().ToString();
                user.SecurityStamp = Guid.NewGuid().ToString();
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.UserSettings.AddAsync(new UserSetting
                {
                    UserId = user.Id,
                    Culture = "en-US",
                    DarkMode = false,
                    RightToLeft = false,
                });
                var result = await _unitOfWork.SaveChangesAsync();
                await _userManager.AddToRoleAsync(user, ApplicationRoles.User);
                if (_registerConfirmationRequired)
                {
                    try
                    {
                        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        user.EmailVerificationToken = _tokenService.GenerateRandomCode();
                        user.EmailTokenExpires = DateTime.UtcNow.AddHours(1);
                        await _smtpService.SendEmailAsync(user.Email, $"درخواست تایید ایمیل کاربر {user.Email}", user.EmailVerificationToken, true);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                        user.PhoneNumberConfirmed = true;
                    else
                        user.PhoneNumberConfirmed = false;

                    user.EmailConfirmed = true;
                }
                await _unitOfWork.SaveChangesAsync();

                return Ok(user.Id.ToString());



            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }

        }
        /// <summary>
        /// Add New User by email and with password. return newly added userId as string
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string</returns>
        [HttpPost("NewUser")]
        [Authorize(Policy = ApplicationPermissions.User.Create)]
        public async Task<IActionResult> AddNewUserWithPassword(NewUserRequest request)
        {

            try
            {

                if (await _unitOfWork.Users.AnyAsync(a => a.Email.Equals(request.Email)))
                    return Conflict(await Result.FailAsync("Email is is already exist."));

                var user = _mapper.Map<User>(request);
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _unitOfWork.UserSettings.AddAsync(new UserSetting
                    {
                        UserId = user.Id,
                        Culture = "en-US",
                        DarkMode = false,
                        RightToLeft = false,
                    });
                    await _userManager.AddToRoleAsync(user, ApplicationRoles.User);

                    if (_registerConfirmationRequired)
                    {
                        try
                        {
                            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            user.EmailVerificationToken = _tokenServices.GenerateRandomCode();
                            user.EmailTokenExpires = DateTime.UtcNow.AddHours(1);
                            await _smtpService.SendEmailAsync(user.Email, $"درخواست تایید ایمیل کاربر {user.Email}", user.EmailVerificationToken, true);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                            user.PhoneNumberConfirmed = true;
                        else
                            user.PhoneNumberConfirmed = false;

                        user.EmailConfirmed = true;
                    }
                    await _unitOfWork.SaveChangesAsync();
                    return Ok(await Result<string>.SuccessAsync(data: user.Id.ToString()));

                }
                else
                {
                    return BadRequest(await Result.FailAsync(result.Errors.Select(x => x.Description).ToList()));
                }

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }

        }


        /// <summary>
        /// Assign role to user by roleId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>

        [HttpPost("{userId}/roles")]
        [Authorize(Policy = ApplicationPermissions.User.Create)]

        public async Task<IActionResult> AddRoleToUserByRoleId(string userId, string roleId)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                if (roleId is null)
                    return BadRequest(await Result.FailAsync("RoleId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                var role = await _unitOfWork.Roles.FindAsync(Guid.Parse(roleId));
                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found."));

                if (await _unitOfWork.UserRoles.AnyAsync(x => x.UserId == user.Id && x.RoleId == role.Id))
                    return Conflict(await Result.FailAsync("Alredy assign."));
                var userRole = new UserRole(Guid.Parse(userId), Guid.Parse(roleId));
                await _unitOfWork.UserRoles.AddAsync(userRole);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Role successfully assigned to user."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Remove role from user by roleId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpDelete("{userId}/roles")]

        public async Task<IActionResult> RemoveRoleFromUserByRoleId(string userId, string roleId)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                if (roleId is null)
                    return BadRequest(await Result.FailAsync("RoleId is null or empty."));

                var userRole = await _unitOfWork.UserRoles.GetFirstOrDefaultAsync(
                    predicate: x => x.RoleId.ToString().Equals(roleId) && x.UserId.ToString().Equals(userId));

                if (userRole == null)
                    return NotFound(await Result.FailAsync("UserRole not found."));

                _unitOfWork.UserRoles.Remove(userRole);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Role successfully removed."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Get user claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of claims</returns>
        [HttpGet("{userId}/claims")]
        public async Task<IActionResult> GetClaimsByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));


                var claims = await _unitOfWork.UserClaims.GetAllAsync(predicate: x => x.UserId == user.Id,
                      selector: s => new ClaimResponse
                      {
                          ClaimType = s.ClaimType,
                          ClaimValue = s.ClaimValue,
                      });

                return Ok(await Result<IList<ClaimResponse>>.SuccessAsync(claims));

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get user active sessions
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of UserSessionResponse</returns>
        [HttpGet("{userId}/sessions")]
        public async Task<IActionResult> GetActiveSessionsByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));


                var userSession = await _unitOfWork.UserSessions.GetAllAsync(predicate: x => x.UserId == user.Id,
                      selector: SelectExpressions.UserSessions.UserSessionResponse);

                return Ok(await Result<IList<UserSessionResponse>>.SuccessAsync(userSession));

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of RoleResponse</returns>
        [HttpGet("{userId}/Roles")]
        public async Task<IActionResult> GetRolesByUserId(string userId)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                var roles = await _unitOfWork.UserRoles.GetAllAsync(predicate: x => x.UserId == user.Id,
                    selector: SelectExpressions.UserRoles.RoleResponse);
                return Ok(await Result<List<RoleResponse>>.SuccessAsync(roles.ToList()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get user tenants
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of ClientResponse</returns>
        [HttpGet("{userId}/tenants")]
        public async Task<IActionResult> GetTenantsByUserId(string userId)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                var tenants = await _unitOfWork.UserTenants.GetAllAsync(predicate: x => x.UserId == user.Id,
                    selector: SelectExpressions.UserTenants.TenantResponse);

                return Ok(await Result<List<TenantResponse>>.SuccessAsync(tenants.ToList()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Delete user by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>OK</returns>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserById(string userId)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                _unitOfWork.Users.Remove(user);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("User successfully deleted"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Terminate user session by sessionId
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns>OK(bool)</returns>
        [HttpDelete("sessions/{sessionId}")]
        public async Task<IActionResult> TerminateSession(string sessionId)
        {
            try
            {
                if (sessionId is null)
                    return BadRequest(await Result.FailAsync("SessionId is null or empty."));

                var session = await _unitOfWork.UserSessions.FindAsync(Guid.Parse(sessionId));
                if (session is null)
                    return NotFound(await Result.FailAsync("Session not found."));

                _unitOfWork.UserSessions.Remove(session);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result<bool>.SuccessAsync(true,"Session successfully Terminated."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Get list of users
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="paged"></param>
        /// <returns>List or PagedList of UserResponse</returns>
        [HttpGet()]
        public async Task<IActionResult> GetUsers(string? query = null, int page = 0, int pageSize = 10, bool paged = true)
        {
            try
            {
                if (paged)
                {
                    var users = await _unitOfWork.Users.GetPagedListAsync(selector: SelectExpressions.Users.UserResponse,
                    predicate: x => ((!string.IsNullOrWhiteSpace(query) ? x.Email.Contains(query) : true)
                    || (!string.IsNullOrWhiteSpace(query) ? x.FirstName.Contains(query) : true)
                    || (!string.IsNullOrWhiteSpace(query) ? x.LastName.Contains(query) : true)
                    || (!string.IsNullOrWhiteSpace(query) ? x.PhoneNumber.Contains(query) : true)),
                    orderBy: o => o.OrderByDescending(k => k.CreatedDate),
                    pageIndex: Convert.ToInt32(page),
                    pageSize: Convert.ToInt32(pageSize) > 100 ? 100 : Convert.ToInt32(pageSize));
                    return Ok(await Result<IPagedList<UserResponse>>.SuccessAsync(users));

                }
                else
                {
                    var users = await _unitOfWork.Users.GetAllAsync(selector: SelectExpressions.Users.UserResponse,
                predicate: x => ((!string.IsNullOrWhiteSpace(query) ? x.Email.Contains(query) : true)
                || (!string.IsNullOrWhiteSpace(query) ? x.FirstName.Contains(query) : true)
                || (!string.IsNullOrWhiteSpace(query) ? x.LastName.Contains(query) : true)
                || (!string.IsNullOrWhiteSpace(query) ? x.PhoneNumber.Contains(query) : true)),
                orderBy: o => o.OrderByDescending(k => k.CreatedDate));
                    return Ok(await Result<IList<UserResponse>>.SuccessAsync(users));
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>UserResponse</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {

                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Id.ToString().Equals(userId),
                    selector: SelectExpressions.Users.UserResponse);
                var a = user.Id.ToString();
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                return Ok(await Result<UserResponse>.SuccessAsync(user));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get user by jwt
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns>UserResponse</returns>
        [HttpPost("Token")]
        public async Task<IActionResult> GetUserByJWT([FromBody] string jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken))
                    return BadRequest(await Result.FailAsync("Token is null or empty."));

                var principal = _tokenService.ValidateToken(jwtToken);
                var userId = principal?.FindFirst(x => x.Type.Equals(ApplicationClaimTypes.UserId));
                if (userId is null)
                    return BadRequest(await Result.FailAsync("Invalid Token."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(
                    predicate: x => x.Id.ToString().Equals(userId.Value),
                    selector: SelectExpressions.Users.UserResponse);

                if (user == null)
                    return NotFound(await Result.FailAsync("User not found."));

                return Ok(await Result<UserResponse>.SuccessAsync(user));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get user claims by jwt
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns>List of UserClaims</returns>
        [HttpPost("claims")]
        public async Task<IActionResult> GetUserClaimsByJWT([FromBody] string jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken))
                    return BadRequest(await Result.FailAsync("Token is null or empty."));

                var principal = _tokenService.ValidateToken(jwtToken);
                var userId = principal?.FindFirst(x => x.Type.Equals(ApplicationClaimTypes.UserId));
                if (userId is null)
                    return BadRequest(await Result.FailAsync("Invalid Token."));


                var claims = await _unitOfWork.UserClaims.GetAllAsync(
                    predicate: x => x.UserId.ToString().Equals(userId.Value),
                    selector: s => new UserClaimResponse
                    {
                        ClaimType = s.ClaimType,
                        ClaimValue = s.ClaimValue,
                    });

                return Ok(await Result<IList<UserClaimResponse>>.SuccessAsync(claims));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get user tenants by jwt
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns>List of ClientResponse</returns>
        [HttpPost("tenants")]
        public async Task<IActionResult> GetUserTenantsByJWT([FromBody] string jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken))
                    return BadRequest(await Result.FailAsync("Token is null or empty."));

                var principal = _tokenService.ValidateToken(jwtToken);
                var userId = principal?.FindFirst(x => x.Type .Equals( ApplicationClaimTypes.UserId));
                if (userId is null)
                    return BadRequest(await Result.FailAsync("Invalid Token."));


                var tenants = await _unitOfWork.UserTenants.GetAllAsync(
                    predicate: x => x.UserId.ToString().Equals(userId.Value),
                    selector: SelectExpressions.UserTenants.TenantResponse);
                return Ok(await Result<IList<TenantResponse>>.SuccessAsync(tenants));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get user clients by jwt
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns>List of ClientResponse</returns>
        [HttpPost("roles")]
        public async Task<IActionResult> GetUserRolesByJWT([FromBody] string jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken))
                    return BadRequest(await Result.FailAsync("Token is null or empty."));

                var principal = _tokenService.ValidateToken(jwtToken);
                var userId = principal?.FindFirst(x => x.Type .Equals( ApplicationClaimTypes.UserId));
                if (userId is null)
                    return BadRequest(await Result.FailAsync("Invalid Token."));


                var roles = await _unitOfWork.UserRoles.GetAllAsync(
                    predicate: x => x.UserId .ToString().Equals(userId.Value),
                    selector: SelectExpressions.UserRoles.RoleResponse);
                return Ok(await Result<IList<RoleResponse>>.SuccessAsync(roles));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get user by refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>UserResponse</returns>
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> GetUserByRefreshToken([FromBody] string refreshToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                    return BadRequest(await Result.FailAsync("Token is null or empty."));

                var principal = _tokenService.ValidateToken(refreshToken);
                var userId = principal?.FindFirst(x => x.Type .Equals( ApplicationClaimTypes.UserId));
                if (userId is null)
                    return BadRequest(await Result.FailAsync("Invalid Token."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Id .ToString().Equals(userId.Value),
                    selector: SelectExpressions.Users.UserResponse);
                if (user == null)
                    return NotFound(await Result.FailAsync("User not found."));

                return Ok(await Result<UserResponse>.SuccessAsync(user));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// update user by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{userId}")]
        public async Task<IActionResult> UpdateUserById(string userId, [FromBody] JsonPatchDocument<UserRequest> request)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                var requestToPatch = _mapper.Map<UserRequest>(user);
                request.ApplyTo(requestToPatch);
                _mapper.Map(requestToPatch, user);

                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("User updated successfully."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{userId}/change-password")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

                if (result.Succeeded)
                {
                    return Ok(await Result.SuccessAsync("Password successfully changed."));
                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(result.Errors.Select(x => x.Description).ToList()));

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// replace user Tenants
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{userId}/UpdateTenants")]
        public async Task<IActionResult> UpdateUserTenants(string userId, [FromBody] List<string> request)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                _unitOfWork.UserTenants.RemoveRange(predicate: x => x.UserId == user.Id);
                foreach (var item in request)
                {
                    if (item is not null)
                        await _unitOfWork.UserTenants.AddAsync(new UserTenant { UserId = Guid.Parse(userId), TenantId = Guid.Parse(item) });
                }
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("User clients successfully replaced."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// replace user clients
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{userId}/UpdateClaims")]
        public async Task<IActionResult> UpdateUserClaims(string userId, [FromBody] List<UserClaimRequest> request)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                _unitOfWork.UserClaims.RemoveRange(predicate: x => x.UserId == user.Id);
                foreach (var item in request)
                {
                    if (item is not null)
                        await _unitOfWork.UserClaims.AddAsync(new UserClaim { UserId = Guid.Parse(userId), ClaimType = item.ClaimType, ClaimValue = item.ClaimValue });
                }
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("User claims successfully updated."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Replace user roles 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{userId}/UpdateRoles")]
        public async Task<IActionResult> UpdateUserRoles(string userId, [FromBody] List<string> request)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result.FailAsync("User not found."));

                _unitOfWork.UserRoles.RemoveRange(predicate: x => x.UserId == user.Id);
                foreach (var item in request)
                {
                    if (item is not null)
                        await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = Guid.Parse(userId), RoleId = Guid.Parse(item) });
                }
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("User roles successfully replaced."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


    }
}
