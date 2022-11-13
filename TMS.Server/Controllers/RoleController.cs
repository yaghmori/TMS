using AutoMapper;
using Infrastructure.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TMS.Core.Domain.Entities;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.Constants;
using TMS.Shared.PagedCollections;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;
using TMS.Web.Server.Extensions;

namespace TMS.API.Controllers
{
    [Authorize]
    [Route("api/v1/server/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<ServerDbContext> _unitOfWork;

        public RoleController(IMapper mapper, ServerDbContext context, IUnitOfWork<ServerDbContext> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Add Role by Name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>RoleId as string</returns>
        [HttpPost("{roleName}")]
        [Authorize(Policy = ApplicationPermissions.Roles.Create)]
        public async Task<IActionResult> AddRole(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    return BadRequest(await Result.FailAsync("RoleName is null or empty."));
                if (await _unitOfWork.Roles.AnyAsync(x => x.Name.Equals(roleName)))
                    return Conflict(await Result.FailAsync("The role name is already defined."));
                var role = new Role(roleName);
                role.NormalizedName = roleName.Normalize().ToUpper();
                await _unitOfWork.Roles.AddAsync(role);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result<string>.SuccessAsync(data: role.Id.ToString()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Delete Role by Name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpDelete("Name/{roleName}")]
        [Authorize(Policy = ApplicationPermissions.Roles.Delete)]
        public async Task<IActionResult> DeleteRoleByName(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    return BadRequest(await Result.FailAsync("RoleName is null or empty."));

                var role = await _unitOfWork.Roles.GetFirstOrDefaultAsync(predicate: x => x.Name.Equals(roleName));
                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found."));

                _unitOfWork.Roles.Remove(role);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Role successfully deleted."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Dlete Role by Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>OK</returns>
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRoleById(string roleId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    return BadRequest(await Result.FailAsync("RoleId is null or empty."));

                var role = await _unitOfWork.Roles.FindAsync(Guid.Parse(roleId));
                if (role is null) NotFound(await Result.FailAsync("Role not found."));

                _unitOfWork.Roles.Remove(role);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Role successfully deleted."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Update Role by Id 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleName"></param>
        /// <returns>OK</returns>
        [HttpPatch("{roleId}")]
        public async Task<IActionResult> UpdateRoleById(string roleId, string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    return BadRequest(await Result.FailAsync("RoleId is null or empty."));

                if (string.IsNullOrWhiteSpace(roleName))
                    return BadRequest(await Result.FailAsync("RoleName is null or empty."));

                var role = await _unitOfWork.Roles.FindAsync(Guid.Parse(roleId));
                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found"));

                role.Name = roleName;
                role.NormalizedName = roleName.Normalize().ToUpper();
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Role updated successfully."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get All Roles 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="paged"></param>
        /// <returns>List of RoleResponse</returns>
        [HttpGet]
        public async Task<IActionResult> GetRoles(string? query = null, int page = 0, int pageSize = 10, bool paged = true)
        {
            try
            {
                if (paged)
                {
                    var clients = await _unitOfWork.Roles.GetPagedListAsync(selector: SelectExpressions.Roles.DetailedRoleResponse,
                        include: x => x.Include(i => i.UserRoles).ThenInclude(i => i.User).Include(i => i.RoleClaims),
                    predicate: x => ((!string.IsNullOrWhiteSpace(query) ? x.NormalizedName.Contains(query.Normalize().ToUpper()) : true)),
                    orderBy: o => o.OrderByDescending(k => k.CreatedDate),
                    pageIndex: Convert.ToInt32(page),
                    pageSize: Convert.ToInt32(pageSize) > 100 ? 100 : Convert.ToInt32(pageSize));
                    var a = await Result<IPagedList<RoleResponse>>.SuccessAsync(clients);
                    return Ok(a);

                }
                else
                {
                    var clients = await _unitOfWork.Roles.GetAll(selector: SelectExpressions.Roles.DetailedRoleResponse,
                        include: x => x.Include(i => i.UserRoles).ThenInclude(i => i.User).Include(i => i.RoleClaims),
                    predicate: x => ((!string.IsNullOrWhiteSpace(query) ? x.NormalizedName.Contains(query.Normalize().ToUpper()) : true)),
                    orderBy: o => o.OrderByDescending(k => k.CreatedDate)).ToListAsync();
                    return Ok(await Result<List<RoleResponse>>.SuccessAsync(clients));
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get Role by Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>RoleResponse</returns>
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    return BadRequest(await Result.FailAsync("RoleId is null or empty."));

                var role = await _unitOfWork.Roles.GetFirstOrDefaultAsync(selector: SelectExpressions.Roles.DetailedRoleResponse,
                    include: x => x.Include(i => i.UserRoles).ThenInclude(i => i.User).Include(i => i.RoleClaims),
                predicate: x => x.Id.ToString().Equals(roleId));

                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found"));

                return Ok(await Result<RoleResponse>.SuccessAsync(role));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get Role by Name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>RoleResponse</returns>
        [HttpGet("Name/{roleName}")]
        public async Task<IActionResult> GetRoleByName(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    return BadRequest(await Result.FailAsync("RoleName is null or empty."));

                var role = await _unitOfWork.Roles.GetFirstOrDefaultAsync(selector: SelectExpressions.Roles.DetailedRoleResponse,
                    include: x => x.Include(i => i.UserRoles).ThenInclude(i => i.User).Include(i => i.RoleClaims),
                predicate: x => x.Name.Equals(roleName));

                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found"));

                return Ok(await Result<RoleResponse>.SuccessAsync(role));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get users by RoleName
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>List of UserResponse</returns>

        [HttpGet("Name/{roleName}/users")]
        public async Task<IActionResult> GetUsersByRoleName(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    return BadRequest(await Result.FailAsync("RoleName is null or empty."));

                var role = await _unitOfWork.Roles.GetFirstOrDefaultAsync(predicate: x => x.Name.Equals(roleName));

                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found"));

                var users = await _unitOfWork.UserRoles.GetAllAsync(predicate: x => x.RoleId==role.Id,
                    selector: SelectExpressions.UserRoles.UserResponse);

                return Ok(await Result<List<UserResponse>>.SuccessAsync(users.ToList()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get users by RoleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>List of UserResponse</returns>
        [HttpGet("{roleId}/users")]
        public async Task<IActionResult> GetUsersByRoleId(string roleId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    return BadRequest(await Result.FailAsync("RoleId is null or empty."));

                var role = await _unitOfWork.Roles.FindAsync(Guid.Parse(roleId));
                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found"));

                var users = await _unitOfWork.UserRoles.GetAllAsync(predicate: x => x.RoleId == role.Id,
                    selector: SelectExpressions.UserRoles.UserResponse);

                return Ok(await Result<List<UserResponse>>.SuccessAsync(users.ToList()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get role claims by RoleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>List of claimResponse</returns>
        [HttpGet("{roleId}/claims")]
        public async Task<IActionResult> GetRoleClaimsByRoleId(string roleId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    return BadRequest(await Result.FailAsync("RoleId is null or empty."));

                var role = await _unitOfWork.Roles.FindAsync(Guid.Parse(roleId));
                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found"));

                var claims = await _unitOfWork.RoleClaims.GetAllAsync(
                     predicate: x => x.RoleId.ToString().Equals(roleId),
                    selector: SelectExpressions.RoleClaims.ClaimResponse);


                return Ok(await Result<List<ClaimResponse>>.SuccessAsync(claims.ToList()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Replace users by RoleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="request"></param>
        /// <returns>OK</returns>
        [HttpPost("{roleId}/Users")]
        public async Task<IActionResult> ReplaceUsersByRoleId(string roleId, [FromBody] List<string> request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    return BadRequest(await Result.FailAsync("RoleId is null or empty."));

                var role = await _unitOfWork.Roles.FindAsync(Guid.Parse(roleId));
                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found."));

                _unitOfWork.UserRoles.RemoveRange(predicate: x => x.RoleId == role.Id);
                foreach (var item in request)
                {
                    if (item is not null)
                        await _unitOfWork.UserRoles.AddAsync(new UserRole { UserId = Guid.Parse(item), RoleId = role.Id });
                }
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Users replaced successfully."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Replace role claims
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="request"></param>
        /// <returns>OK</returns>
        [HttpPost("{roleId}/claims")]
        public async Task<IActionResult> UpdateRoleClaims(string roleId, [FromBody] List<ClaimResponse> request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleId))
                    return BadRequest(await Result.FailAsync("RoleId is null or empty."));

                var role = await _unitOfWork.Roles.FindAsync(Guid.Parse(roleId));
                if (role is null)
                    return NotFound(await Result.FailAsync("Role not found."));

                _unitOfWork.RoleClaims.RemoveRange(predicate: x => x.RoleId == role.Id);
                foreach (var item in request)
                {
                    if (item is not null)
                        await _unitOfWork.RoleClaims.AddAsync(new RoleClaim { RoleId = role.Id, ClaimType = item.ClaimType, ClaimValue = item.ClaimValue });
                }
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("RoleClaims updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }
    }

}

