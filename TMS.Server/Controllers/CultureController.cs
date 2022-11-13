using AutoMapper;
using Infrastructure.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Core.Application;
using TMS.Core.Domain.Entities;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;
using TMS.Web.Server.Extensions;

namespace TMS.API.Controllers
{
    [Route("api/v1/server/cultures")]
    [ApiController]
    public class CultureController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<ServerDbContext> _unitOfWork;

        public CultureController(IMapper mapper, IUnitOfWork<ServerDbContext> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Get All Cultures
        /// </summary>
        /// <returns>List of CultureResponse</returns>
        [HttpGet]
        public async Task<IActionResult> GetCultures()
        {
            try
            {
                var response = await _unitOfWork.Cultures.GetAllAsync(selector: SelectExpressions.Cultures.CultureResponse);
                //var cultures = _mapper.Map<List<CultureResponse>>(response);
                return Ok(await Result<List<CultureResponse>>.SuccessAsync((List<CultureResponse>)response));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Get culture by id
        /// </summary>
        /// <param name="cultureId"></param>
        /// <returns>List of CultureResponse</returns>
        [HttpGet("{cultureId}")]
        public async Task<IActionResult> GetCultureById(string cultureId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cultureId)) 
                    return BadRequest(await Result.FailAsync("CultureId is null or empty."));

                var result = await _unitOfWork.Cultures.GetFirstOrDefaultAsync(predicate: x => x.Id.ToString().Equals(cultureId));
                if (result == null) 
                    return NotFound(await Result.FailAsync("Culture not found."));

                return Ok(await Result<CultureResponse>.SuccessAsync(_mapper.Map<CultureResponse>(result)));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Add new culture
        /// </summary>
        /// <param name="request"></param>
        /// <returns>string(CultureId)</returns>
        [HttpPost]
        public async Task<IActionResult> AddCulture(CultureRequest request)
        {
            try
            {
                if (await _unitOfWork.Cultures.AnyAsync(x => x.CultureName.Equals(request.CultureName)))
                    return Conflict(await Result.FailAsync("The culture is already defined."));

                var culture = _mapper.Map<Culture>(request);
                await _unitOfWork.Cultures.AddAsync(culture);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result<string>.SuccessAsync(data: culture.Id.ToString()));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// delete culture by Id
        /// </summary>
        /// <param name="cultureId"></param>
        /// <returns>OK</returns>
        [HttpDelete("{cultureId}")]
        public async Task<IActionResult> DeleteCultureById(string cultureId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cultureId))
                    return BadRequest(await Result.FailAsync("CultureId is null or empty."));

                var culture = await _unitOfWork.Cultures.FindAsync(Guid.Parse(cultureId));
                if (culture is null)
                    return NotFound(await Result.FailAsync("Culture not found."));

                _unitOfWork.Cultures.Remove(culture);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Culture successfully deleted"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Update culture
        /// </summary>
        /// <param name="cultureId"></param>
        /// <param name="request"></param>
        /// <returns>OK</returns>
        [HttpPatch("{cultureId}")]
        public async Task<IActionResult> UpdateCultureById(string cultureId, CultureRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cultureId))
                    return BadRequest(await Result.FailAsync("CultureId is null or empty."));

                var culture = await _unitOfWork.Cultures.FindAsync(Guid.Parse(cultureId));
                if (culture is null)
                    return NotFound(await Result.FailAsync("Culture not found."));

                culture = _mapper.Map<Culture>(request);
                _unitOfWork.Cultures.Update(culture);
                await _unitOfWork.SaveChangesAsync();
                return Ok(await Result.SuccessAsync("Culture successfully updated"));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }








    }
}
