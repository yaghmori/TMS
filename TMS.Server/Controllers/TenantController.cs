using AutoMapper;
using Infrastructure.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Core.Domain.Entities;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.Constants;
using TMS.Shared.PagedCollections;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;
using TMS.Web.Server.Extensions;

namespace TMS.API.Controllers
{
    [Authorize]
    [Route("api/v1/server/tenants")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork<ServerDbContext> _unitOfWork;
        private readonly IUnitOfWork<TenantDbContext> _tenantUnitOfWork;


        public TenantController(IMapper mapper,
            IHostEnvironment hostEnvironment,
            IConfiguration configuration,
            IUnitOfWork<ServerDbContext> hostUnitOfWork,
            IUnitOfWork<TenantDbContext> tenantUnitOfWork)
        {
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
            _unitOfWork = hostUnitOfWork;
            _tenantUnitOfWork = tenantUnitOfWork;
        }

        /// <summary>
        /// Get list of tenants
        /// </summary>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="paged"></param>
        /// <returns>List or paged list of TenantResponse</returns>
        /// 
        [HttpGet()]
        [Authorize(Policy = ApplicationPermissions.Tenants.View)]

        public async Task<IActionResult> GetTenants(string? query = null, int page = 0, int pageSize = 10, bool paged = true)
        {
            try
            {
                if (paged)
                {
                    var tenants = await _unitOfWork.Tenants.GetPagedListAsync(selector: SelectExpressions.Tenants.TenantResponse,
                    predicate: x => ((!string.IsNullOrWhiteSpace(query) ? x.NormalizedName.Contains(query.Normalize()) : true)
                    || (!string.IsNullOrWhiteSpace(query) ? x.ExpireDate.ToString().Contains(query) : true)),
                    orderBy: o => o.OrderByDescending(k => k.CreatedDate),
                    pageIndex: Convert.ToInt32(page),
                    pageSize: Convert.ToInt32(pageSize) > 100 ? 100 : Convert.ToInt32(pageSize));
                    return Ok(await Result<IPagedList<TenantResponse>>.SuccessAsync(tenants));

                }
                else
                {
                    var tenants = await _unitOfWork.Tenants.GetAllAsync(selector: SelectExpressions.Tenants.TenantResponse,
                    predicate: x => ((!string.IsNullOrWhiteSpace(query) ? x.Name.Contains(query) : true)
                    || (!string.IsNullOrWhiteSpace(query) ? x.ExpireDate.ToString().Contains(query) : true)),
                    orderBy: o => o.OrderByDescending(k => k.CreatedDate));
                    return Ok(await Result<IList<TenantResponse>>.SuccessAsync(tenants));
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get tenant by id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>TenantResponse</returns>
        [HttpGet("{tenantId}")]
        [Authorize(Policy = ApplicationPermissions.Tenants.View)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantResponse))]
        public async Task<IActionResult> GetTenantById(string tenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId))
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                var tenant = await _unitOfWork.Tenants.GetFirstOrDefaultAsync(predicate: x => x.Id.ToString().Equals(tenantId),
                    selector: SelectExpressions.Tenants.TenantResponse);

                if (tenant == null)
                    return NotFound(await Result.FailAsync("Tenant not found."));

                return Ok(await Result<TenantResponse>.SuccessAsync(tenant));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get list of users by tenantId
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of UserResponse</returns>
        [HttpGet("{tenantId}/Users")]
        [Authorize(Policy = ApplicationPermissions.Tenants.View)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserResponse>))]
        public async Task<IActionResult> GetUsersByTenantId(string tenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId)) 
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                var tenant = await _unitOfWork.Tenants.FindAsync(Guid.Parse(tenantId));

                if (tenant == null)
                    return NotFound(await Result.FailAsync("Tenant not found."));

                var users = await _unitOfWork.UserTenants.GetAllAsync(predicate: x => x.TenantId.ToString().Equals(tenantId),
                    selector: SelectExpressions.UserTenants.UserResponse);

                return Ok(await Result<IList<UserResponse>>.SuccessAsync(users));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Add new tenant 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string(TenantId)</returns>
        [HttpPost]
        [Authorize(Policy = ApplicationPermissions.Tenants.Create)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> AddTenant(TenantRequest request)
        {
            try
            {
                if (await _unitOfWork.Tenants.AnyAsync(x => x.Name.Equals(request.Name)))
                    return Conflict(await Result.FailAsync("The Name is already defined."));

                var tenant = _mapper.Map<Tenant>(request);
                await _unitOfWork.Tenants.AddAsync(tenant);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result<string>.SuccessAsync(data: tenant.Id.ToString()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Assign user to tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns>string(UserTenantId)</returns>
        [HttpPost("{tenantId}/users/{userId}")]
        [Authorize(Policy = ApplicationPermissions.Tenants.Edit)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> AssignUserToTenant(string tenantId, string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId))
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                if ((await _unitOfWork.Tenants.FindAsync(tenantId)) == null)
                    return NotFound(await Result.FailAsync("The tenant is not found."));

                if ((await _unitOfWork.Users.FindAsync(userId)) == null)
                    return NotFound(await Result.FailAsync("The user is not found."));

                var tenant = await _unitOfWork.UserTenants.AddAsync(new UserTenant { TenantId = Guid.Parse(tenantId), UserId = Guid.Parse(userId) });
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result<string>.SuccessAsync(data: tenant.Entity.Id.ToString()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Remove user from tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{tenantId}/users/{userId}")]
        [Authorize(Policy = ApplicationPermissions.Tenants.Edit)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveUserFromTenant(string tenantId, string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId))
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var userTenant = await _unitOfWork.UserTenants.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId.ToString().Equals(userId) && x.TenantId.ToString().Equals(tenantId));

                if (userTenant == null)
                    return BadRequest(await Result.FailAsync("tenant or user not found."));

                _unitOfWork.UserTenants.Remove(userTenant);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("User has been removed from tenant."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Delete UserTenant by id
        /// </summary>
        /// <param name="UserTenantId">id of userTenant</param>
        /// <returns></returns>
        [HttpDelete("~/api/host/v1/UserTenants/{UserTenantId}")]
        [Authorize(Policy = ApplicationPermissions.Tenants.Edit)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserTenantById(string UserTenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(UserTenantId)) return BadRequest(await Result.FailAsync("UserTenantId is null or empty."));

                var userTenant = await _unitOfWork.UserTenants.FindAsync(Guid.Parse(UserTenantId));
                if (userTenant == null)
                    return NotFound(await Result.FailAsync("UserTenant not found."));

                _unitOfWork.UserTenants.Remove(userTenant);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("UserTenant has been deleted."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Delete tenant by id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpDelete("{tenantId}")]
        [Authorize(Policy = ApplicationPermissions.Tenants.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTenantById(string tenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId)) 
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                var tenant = await _unitOfWork.Tenants.FindAsync(Guid.Parse(tenantId));
                if (tenant is null) 
                    return NotFound(await Result.FailAsync("The Tenant not found."));

                _unitOfWork.Tenants.Remove(tenant);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Tenant successfully deleted"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Update tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{tenantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTenantById(string tenantId,  JsonPatchDocument<TenantRequest> request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId))
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                var tenant = await _unitOfWork.Tenants.FindAsync(Guid.Parse(tenantId));
                if (tenant is null)
                    return NotFound(await Result.FailAsync("The Tenant not found."));

                var requestToPatch = _mapper.Map<TenantRequest>(tenant);
                request.ApplyTo(requestToPatch);
                _mapper.Map(requestToPatch, tenant);

                var result = await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Tenant successfully updated"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Replace tenant users
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{tenantId}/users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ReplaceTenantUsers(string tenantId, [FromBody] List<string> request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId)) 
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                var tenant = await _unitOfWork.Tenants.FindAsync(Guid.Parse(tenantId));
                if (tenant is null)
                    return NotFound(await Result.FailAsync("The Tenant not found."));

                _unitOfWork.UserTenants.RemoveRange(predicate: x => x.TenantId.ToString().Equals(tenantId));
                foreach (var item in request)
                {
                    if (item is not null)
                        await _unitOfWork.UserTenants.AddAsync(new UserTenant { UserId = Guid.Parse(item), TenantId = tenant.Id });
                }
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Users successfully replaced."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Create Database
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpPost("{tenantId}/EnsureCreated")]
        [Authorize(Policy = ApplicationPermissions.Tenants.Create)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateDatabase(string tenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId))
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                var tenant = await _unitOfWork.Tenants.FindAsync(Guid.Parse(tenantId));
                if (tenant is null)
                    return NotFound(await Result.FailAsync("Tenant not found."));

                tenant.ConnectionString = string.Format(tenant.ConnectionString, tenantId);
                await _unitOfWork.SaveChangesAsync();
                _tenantUnitOfWork.DbContext.Database.SetConnectionString(tenant.ConnectionString);
                var result = await _tenantUnitOfWork.DbContext.Database.EnsureCreatedAsync();
                if (result)
                    return Ok(await Result.SuccessAsync("Database created successfully."));
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync("Create Database failed."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Migrate Database
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpPost("{tenantId}/Migrate")]
        [Authorize(Policy = ApplicationPermissions.Tenants.Edit)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MigrateDatabase(string tenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId))
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                var tenant = await _unitOfWork.Tenants.FindAsync(Guid.Parse(tenantId));
                if (tenant is null)
                    return NotFound(await Result.FailAsync("Tenant not found."));

                _tenantUnitOfWork.DbContext.Database.SetConnectionString(tenant.ConnectionString);
                await _tenantUnitOfWork.DbContext.Database.MigrateAsync();
                return Ok(await Result.SuccessAsync("Database migrated successfully."));

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Delete Database
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpDelete("{tenantId}/EnsureDeleted")]
        [Authorize(Policy = ApplicationPermissions.Tenants.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteDatabase(string tenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantId))
                    return BadRequest(await Result.FailAsync("TenantId is null or empty."));

                var tenant = await _unitOfWork.Tenants.FindAsync(Guid.Parse(tenantId));
                if (tenant is null)
                    return NotFound(await Result.FailAsync("Tenant not found."));

                _tenantUnitOfWork.DbContext.Database.SetConnectionString(tenant.ConnectionString);
                var result = await _tenantUnitOfWork.DbContext.Database.EnsureDeletedAsync();
                if (result)
                    return Ok(await Result.SuccessAsync("Database has been deleted."));
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync("Delete Database failed."));

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        private string BuildConnectionString(string tenantId)
        {
            if (_hostEnvironment.IsDevelopment())
                return string.Format(_configuration["ConnectionStrings:Pattern_db_dev"], tenantId);
            else
                return string.Format(_configuration["ConnectionStrings:Pattern_db"], tenantId);
        }
    }
}
