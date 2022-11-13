using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using TMS.Core.Domain.Entities;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.Constants;
using TMS.Shared.Helpers;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;
using TMS.Web.Server.Extensions;
using TMS.Web.Server.Services;

namespace TMS.API.Controllers
{
    [Route("api/v1/server/auth")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISmtpService _messageSender;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenServices;
        private readonly bool _registerConfirmationRequired;
        private readonly IUnitOfWork<ServerDbContext> _unitOfWork;

        public AuthController(ISmtpService messageSender,
            UserManager<User> userManager,
            SignInManager<User> signInManager,

            IMapper mapper, IConfiguration configuration, ITokenService tokenServices, IUnitOfWork<ServerDbContext> unitOfWork)
        {
            _mapper = mapper;
            _messageSender = messageSender;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenServices = tokenServices;
            _registerConfirmationRequired = Convert.ToBoolean(_configuration.GetSection("RegisterConfirmationRequired").Value);
            _unitOfWork = unitOfWork;
        }




        /// <summary>
        /// Refresh expired token
        /// </summary>
        /// <param name="request"></param>
        /// <returns>TokenResponse</returns>
        [HttpPost]
        [Route("token/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(TokenRequest request)
        {
            try
            {
                //Validate refresh token
                var userSession = await _unitOfWork.UserSessions.GetFirstOrDefaultAsync(predicate: x => x.RefreshToken == request.RefreshToken && x.AccessToken == request.Token);
                if (userSession is null)
                    return BadRequest(await Result.FailAsync("Invalid Token."));

                if (userSession.RefreshTokenExpires <= DateTime.UtcNow)
                    return BadRequest(await Result.FailAsync("Refresh Token is expired."));

                //var principal = _tokenServices.ValidateToken(request.RefreshToken);
                //var userId = principal?.FindFirst(x => x.Type.Equals(ApplicationClaimTypes.UserId))?.Value;
                //if (string.IsNullOrWhiteSpace(userId))
                //    return BadRequest(await Result.FailAsync("Invalid Token."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Id == userSession.UserId);
                if (user is null) return
                        BadRequest(await Result.FailAsync("User not found.")); //user deleted!

                userSession.AccessToken = _tokenServices.GenerateJwtToken(user);
                await _unitOfWork.SaveChangesAsync();
                var token = new TokenResponse
                {
                    AccessToken = userSession.AccessToken,
                    RefreshToken = userSession.RefreshToken
                };
                return Ok(await Result<TokenResponse>.SuccessAsync(token));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Login by email
        /// </summary>
        /// <param name="request"></param>
        /// <returns>TokenResponse</returns>
        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken(SignInByEmailRequest request)
        {
            try
            {

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Email.Equals(request.Email),
                    include: x => x.Include(i => i.UserRoles).ThenInclude(i => i.Role));

                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                    return BadRequest(await Result.FailAsync("Incorrect UserName or password."));

                if (!user.IsActive)
                {
                    return BadRequest(await Result.FailAsync("User Not Active. Please contact the administrator."));
                }


                var session = await _unitOfWork.UserSessions.GetAllAsync(predicate: x => x.UserId == user.Id);
                //Limit User only 1 active session at a time 
                if (session.Any())
                    return Conflict(await Result.FailAsync("Only 1 active session allowed at a time."));

                #region GenerateToken

                var userSession = new UserSession
                {
                    UserId = user.Id,
                    Name = Request.Headers.UserAgent.ToString(),
                    LoginProvider = "IdentityServer",
                    SessionIpAddress = HttpContext.Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    StartDate = DateTime.UtcNow,
                    BuildVersion ="v1",
                    AccessToken = _tokenServices.GenerateJwtToken(user),
                    RefreshToken = Guid.NewGuid().ToString(),
                    RefreshTokenExpires = DateTime.UtcNow.AddDays(Convert.ToInt32(ConfigHelper.JwtSettings.RefreshTokenExpiryInDay)),
                };
                await _unitOfWork.UserSessions.AddAsync(userSession);
                await _unitOfWork.SaveChangesAsync();

                var token = new TokenResponse
                {
                    AccessToken = userSession.AccessToken,
                    RefreshToken = userSession.RefreshToken
                };



                #endregion

                return Ok(await Result<TokenResponse>.SuccessAsync(token));

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }

        }

        /// <summary>
        /// Register email and password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>TokenResponse</returns>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
        {

            try
            {
                var ip = Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                var uid = User?.FindFirst(x => x.Type.Equals(ApplicationClaimTypes.UserId))?.Value;

                if (await _unitOfWork.Users.AnyAsync(a => a.Email.Equals(request.NormalizedEmail)))
                    return Conflict(await Result<TokenResponse>.FailAsync("Email is is already exist."));

                var user = _mapper.Map<User>(request);
                user.UserName = user.Email;
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
                            await _messageSender.SendEmailAsync(user.Email, $"درخواست تایید ایمیل کاربر {user.Email}", user.EmailVerificationToken, true);
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
                    user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Id == user.Id);

                    return Ok(await Result<string>.SuccessAsync(user.Id.ToString()));

                }
                else
                {
                    return BadRequest(await Result<TokenResponse>.FailAsync(result.Errors.Select(x => x.Description).ToList()));
                }

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }

        }

        /// <summary>
        /// Register by Email (Two Factor)
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("RegisterByEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterByEmail(string email)
        {
            try
            {
                if (email is null)
                    return BadRequest(await Result<TokenResponse>.FailAsync("Email is null or empty."));

                if (await _unitOfWork.Users.AnyAsync(a => a.Email.Equals(email)))
                    return Conflict(await Result<TokenResponse>.FailAsync("The entered email is already registered."));

                var user = new User()
                {
                    Email = email,
                    UserName = email,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.UserSettings.AddAsync(new UserSetting
                {
                    UserId = user.Id,
                    Culture = "en-US",
                    DarkMode = false,
                    RightToLeft = false,
                });
                await _unitOfWork.SaveChangesAsync();
                await _userManager.AddToRoleAsync(user, ApplicationRoles.User);

                if (_registerConfirmationRequired)
                {
                    try
                    {
                        user.EmailVerificationToken = _tokenServices.GenerateRandomCode();
                        user.EmailTokenExpires = DateTime.UtcNow.AddHours(1);
                        await _messageSender.SendEmailAsync(user.Email, $"درخواست تایید ایمیل کاربر {user.Email}", user.EmailVerificationToken, true);
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
                user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Id == user.Id,
                    include: x => x.Include(i => i.UserRoles).ThenInclude(i => i.Role));
                if (user == null)
                    return NotFound(await Result.FailAsync("User not found."));

                var token = new TokenResponse
                {
                    AccessToken = _tokenServices.GenerateJwtToken(user),
                    RefreshToken = _tokenServices.GenerateRefreshToken(user)
                };
                return Ok(await Result<TokenResponse>.SuccessAsync(token));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Register by phone number (Two Factor)
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpPost("RegisterByPhoneNumber")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterByPhoneNumber(string phoneNumber)
        {
            try
            {
                if (phoneNumber is null)
                    return BadRequest(await Result<TokenResponse>.FailAsync("PhoneNumber is null or empty."));

                var exist = await _unitOfWork.Users.AnyAsync(a => a.PhoneNumber.Equals(phoneNumber));

                if (exist)
                    return Conflict(await Result<TokenResponse>.FailAsync("The entered phone number is already registered."));

                var user = new User()
                {
                    PhoneNumber = phoneNumber,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString()
                };
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
                        user.PhoneNumberVerificationToken = _tokenServices.GenerateRandomCode();
                        user.PhoneNumberTokenExpires = DateTime.UtcNow.AddHours(1);
                        await _messageSender.SendEmailAsync(user.Email, $"درخواست تایید شماره تلفن کاربر {user.PhoneNumber}", user.PhoneNumberVerificationToken, true);
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
                user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Id == user.Id,
                    include: x => x.Include(i => i.UserRoles).ThenInclude(i => i.Role));
                if (user == null)
                    return NotFound(await Result<TokenResponse>.FailAsync("User not found."));

                var token = new TokenResponse
                {
                    AccessToken = _tokenServices.GenerateJwtToken(user),
                    RefreshToken = _tokenServices.GenerateRefreshToken(user)
                };
                return Ok(await Result<TokenResponse>.SuccessAsync(token));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Check for username is exist 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>bool</returns>
        [HttpPost("UserName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IActionResult> IsExistUserName(string userName)
        {
            try
            {
                if (userName is null)
                    return BadRequest(await Result<bool>.FailAsync("UserName is null or empty."));

                return Ok(await Result<bool>.SuccessAsync(await _unitOfWork.Users.AnyAsync(x => x.UserName.Equals(userName))));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }

        }

        /// <summary>
        /// Check for email is exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns>bool</returns>
        [HttpPost("Email")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IActionResult> IsExistEmail(string email)
        {
            try
            {
                if (email is null)
                    return BadRequest(await Result<bool>.FailAsync("Email is null or empty."));

                return Ok(await Result<bool>.SuccessAsync(await _unitOfWork.Users.AnyAsync(x => x.Email.Equals(email))));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }

        }

        /// <summary>
        /// Check for phone number is exist
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>bool</returns>
        [HttpPost("PhoneNumber")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IActionResult> IsExistPhonenumber(string phoneNumber)
        {
            try
            {
                if (phoneNumber is null)
                    return BadRequest(await Result<bool>.FailAsync("PhoneNumber is null or empty."));

                return Ok(await Result<bool>.SuccessAsync(await _unitOfWork.Users.AnyAsync(x => x.PhoneNumber.Equals(phoneNumber))));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }

        }

        /// <summary>
        /// verify phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="token"></param>
        /// <returns>TokenResponse</returns>
        [HttpPost("Verify-PhoneNumber")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber, string token)
        {
            try
            {
                if (phoneNumber is null)
                    return BadRequest(await Result<TokenResponse>.FailAsync("PhoneNumber is null or empty."));
                if (token is null)
                    return BadRequest(await Result<TokenResponse>.FailAsync("Token is null or empty."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.PhoneNumber.Equals(phoneNumber) && x.PhoneNumberVerificationToken == token);
                if (user is null)
                {
                    return BadRequest(await Result<TokenResponse>.FailAsync("Invalid Token."));
                }
                if (user.PhoneNumberTokenExpires < DateTime.UtcNow)
                {
                    return BadRequest(await Result<TokenResponse>.FailAsync("Token Expired."));
                }

                user.PhoneNumberVerificationToken = null;
                user.PhoneNumberConfirmed = true;
                user.PhoneNumberVerifiedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                var tokenResponse = new TokenResponse
                {
                    AccessToken = _tokenServices.GenerateJwtToken(user),
                    RefreshToken = _tokenServices.GenerateRefreshToken(user)
                };
                return Ok(await Result<TokenResponse>.SuccessAsync(tokenResponse));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }

        }

        /// <summary>
        /// Verify Email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns>TokenResponse</returns>
        [HttpPost("Verify-Email")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            try
            {
                if (email is null) return BadRequest(await Result<TokenResponse>.FailAsync("Email is null or empty."));
                if (token is null) return BadRequest(await Result<TokenResponse>.FailAsync("Token is null or empty."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Email.Equals(email) && x.EmailVerificationToken == token);
                if (user is null)
                {
                    return BadRequest(await Result<TokenResponse>.FailAsync("Invalid Token."));
                }
                if (user.EmailTokenExpires < DateTime.UtcNow)
                {
                    return BadRequest(await Result<TokenResponse>.FailAsync("Token Expired."));
                }

                user.EmailVerificationToken = null;
                user.EmailConfirmed = true;
                user.EmailVerifiedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
                var tokenResponse = new TokenResponse
                {
                    AccessToken = _tokenServices.GenerateJwtToken(user),
                    RefreshToken = _tokenServices.GenerateRefreshToken(user)
                };
                return Ok(await Result<TokenResponse>.SuccessAsync(tokenResponse));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Login by phone number (Two Factor) - send token on sms if phone number exist
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpPost("Login/TwoFactor/PhoneNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> TwoFactorLoginByPhoneNumber(string phoneNumber)
        {
            try
            {
                if (phoneNumber is null) return BadRequest(await Result.FailAsync("PhoneNumber is null or empty."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.PhoneNumber.Equals(phoneNumber));
                if (user is not null)
                {
                    user.PhoneNumberVerificationToken = _tokenServices.GenerateRandomCode();
                    user.PhoneNumberTokenExpires = DateTime.UtcNow.AddHours(1);
                    await _messageSender.SendEmailAsync(user.Email, $"کد ورود به برنامه شماره {user.PhoneNumber}", user.PhoneNumberVerificationToken, true);
                    await _unitOfWork.SaveChangesAsync();
                }
                return Ok(await Result.SuccessAsync("Verification token sent."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Login by email (Two Factor) - send token on email if email exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Login/TwoFactor/Email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> TwoFactorLoginByEmail(string email)
        {
            try
            {
                if (email is null) return BadRequest(await Result.FailAsync("Email is null or empty."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Email.Equals(email));
                if (user is not null)
                {
                    user.PhoneNumberVerificationToken = _tokenServices.GenerateRandomCode();
                    user.PhoneNumberTokenExpires = DateTime.UtcNow.AddHours(1);
                    await _messageSender.SendEmailAsync(user.Email, $"کد ورود به برنامه ایمیل {user.Email}", user.PhoneNumberVerificationToken, true);
                    await _unitOfWork.SaveChangesAsync();
                }
                return Ok(await Result.SuccessAsync("Verification token sent."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Logut
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok(await Result.SuccessAsync());
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Send forgot password token via email
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("{userId}/Forgot-Password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordByUserId(string userId)
        {
            try
            {
                if (userId is null)
                    return BadRequest(await Result.FailAsync("UserId is null or empty."));
                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is not null)
                {
                    user.PasswordResetToken = _tokenServices.GenerateRandomCode();
                    user.PasswordTokenExpires = DateTime.UtcNow.AddHours(1);
                    await _messageSender.SendEmailAsync(user.Email, $"بازیابی کلمه عبور کاربر {user.UserName}", user.PasswordResetToken, true);
                    await _unitOfWork.SaveChangesAsync();
                }
                return Ok(await Result.SuccessAsync("Verification token sent."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Send forgot password token via phoneNumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpPost("Forgot-Password/PhoneNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordByPhoneNumber(string phoneNumber)
        {
            try
            {
                if (phoneNumber is null) return BadRequest(await Result.FailAsync("PhoneNumber is null or empty."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.PhoneNumber.Equals(phoneNumber));
                if (user is not null)
                {
                    user.PasswordResetToken = _tokenServices.GenerateRandomCode();
                    user.PasswordTokenExpires = DateTime.UtcNow.AddHours(1);
                    await _messageSender.SendEmailAsync(user.Email, $"بازیابی کلمه عبور شماره {user.PhoneNumber}", user.PasswordResetToken, true);
                    await _unitOfWork.SaveChangesAsync();
                }
                return Ok(await Result.SuccessAsync("Verification token sent."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Send forgot password token via email 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost("Forgot-Password/UserName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordByUserName(string userName)
        {
            try
            {
                if (userName is null) return BadRequest(await Result.FailAsync("UserName is null or empty."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: u => u.UserName.Equals(userName));
                if (user is not null)
                {
                    user.PasswordResetToken = _tokenServices.GenerateRandomCode();
                    user.PasswordTokenExpires = DateTime.UtcNow.AddHours(1);
                    await _messageSender.SendEmailAsync(user.Email, $"بازیابی کلمه عبور کاربر {user.UserName}", user.PasswordResetToken, true);
                    await _unitOfWork.SaveChangesAsync();
                }
                return Ok(await Result.SuccessAsync("Verification token sent."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// Send forgot password token via email 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Forgot-Password/Email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordByEmail(string email)
        {
            try
            {
                if (email is null) return BadRequest(await Result.FailAsync("UserName is null or empty."));

                var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(predicate: x => x.Email.Equals(email));
                if (user is not null)
                {
                    user.PasswordResetToken = _tokenServices.GenerateRandomCode();
                    user.PasswordTokenExpires = DateTime.UtcNow.AddHours(1);
                    await _messageSender.SendEmailAsync(user.Email, $"بازیابی کلمه عبور ایمیل {user.Email}", user.PasswordResetToken, true);
                    await _unitOfWork.SaveChangesAsync();
                }
                return Ok(await Result.SuccessAsync("Verification token sent."));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }




        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{userId}/Reset-Password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetPassword(string userId, [FromBody] ResetPasswordRequest request)
        {
            try
            {

                if (userId is null) return BadRequest(await Result.FailAsync("UserId is null or empty."));
                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null)
                {
                    return NotFound(await Result.FailAsync("User not found."));
                }
                if (user.PasswordTokenExpires < DateTime.UtcNow)
                {
                    return BadRequest(await Result.FailAsync("Token Expired."));
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, request.Password);

                if (result.Succeeded)
                {
                    user.PasswordResetToken = null;
                    user.PasswordTokenExpires = null;
                    user.PasswordResetedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveChangesAsync();
                    return Ok(await Result.SuccessAsync("Password successfully reset."));
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(result.Errors.Select(x => x.Description).ToList()));

                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }

        /// <summary>
        /// change password 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{userId}/Set-Password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SetPassword(string userId, [FromBody] SetPasswordRequest request)
        {
            try
            {
                if (userId is null) return BadRequest(await Result.FailAsync("UserId is null or empty."));



                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null) return BadRequest(await Result.FailAsync("Invalid Token."));

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, request.Password);

                if (result.Succeeded)
                {
                    await _unitOfWork.SaveChangesAsync();
                    return Ok(await Result.SuccessAsync("Password successfully set."));
                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, await Result<TokenResponse>.FailAsync(result.Errors.Select(x => x.Description).ToList()));

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }


        /// <summary>
        /// Update security stamp
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("{userId}/UpdateSecurityStamp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSecurityStamp(string userId)
        {
            try
            {
                if (userId is null) return BadRequest(await Result.FailAsync("UserId is null or empty."));

                var user = await _unitOfWork.Users.FindAsync(Guid.Parse(userId));
                if (user is null) return NotFound(await Result.FailAsync("User not found."));
                var result = await _userManager.UpdateSecurityStampAsync(user);
                if (result.Succeeded)
                {
                    return Ok(await Result.SuccessAsync("Security stamp updated successfully."));
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, await Result<TokenResponse>.FailAsync(result.Errors.Select(x => x.Description).ToList()));

                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.GetMessages().ToList()));
            }
        }




    }

}

