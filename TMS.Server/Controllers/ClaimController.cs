using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Core.Domain.Entities;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.ResultWrapper;
using TMS.Web.Server.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.API.Controllers
{
    [Authorize]
    [Route("api/v1/server/claims")]
    [ApiController]

    public class ClaimController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<ServerDbContext> _unitOfWork;

        public ClaimController(IMapper mapper, IUnitOfWork<ServerDbContext> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Add new claim to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="claim"></param>
        /// <returns>string</returns>
        [HttpPost("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> AddClaimToUser(string userId, string claimType, string claimValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(await Result<string>.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                    return NotFound(await Result<string>.FailAsync("User not found."));

                var userclaim = new UserClaim(user.Id, claimType, claimValue);
                var result = await _unitOfWork.UserClaims.AddAsync(userclaim);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result<string>.SuccessAsync(data: userclaim.Id.ToString()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get all claims
        /// </summary>
        /// <returns>list of claims</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<UserClaim>))]
        public async Task<IActionResult> GetClaims()
        {
            try
            {
                var claims = await _unitOfWork.UserClaims.GetAllAsync();
                return Ok(await Result<IList<UserClaim>>.SuccessAsync(claims));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Remove UserClaim
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        [HttpDelete]

        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> RemoveClaimFromUser(string userId, [FromBody] Claim claim)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId)) return BadRequest(await Result.FailAsync("UserId is null or empty."));
                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user == null) return NotFound(await Result.FailAsync("User not found."));
                var userClaim = await _unitOfWork.UserClaims.GetFirstOrDefaultAsync(predicate: x => x.UserId == user.Id && x.ClaimType.Equals(claim.Type) && x.ClaimValue.Equals( claim.Value));
                if (userClaim == null) return NotFound(await Result.FailAsync("UserClaim not found."));
                _unitOfWork.UserClaims.Remove(userClaim);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("UserClaim successfully removed."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }


        }



    }
}
