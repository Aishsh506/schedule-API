using AccountBL;
using AccountBL.Models;
using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace MobileMain.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register(RegistrationModel data)
        {
            try
            {
                return Ok(await _accountService.Register(data));
            }
            catch (InvalidPasswordException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
            catch (DataConflictException)
            {
                return Conflict(new ErrorResponse("The email is already used by another account"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost, Route("login")]
        public async Task<IActionResult> Login(LoginModel data)
        {
            try
            {
                var result = await _accountService.Login(data);
                if (result == null)
                {
                    return BadRequest(new ErrorResponse("Incorrect password or email"));
                }
                else return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Incorrect password or email"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost, Route("refresh")]
        public async Task<IActionResult> Refresh(RefreshToken data)
        {
            try
            {
                return Ok(await _accountService.Refresh(data.Token));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Unknown or expired refresh token"));
            }
            catch (InvalidDataException)
            {
                return BadRequest(new ErrorResponse("Invalid refresh token"));
            }
            catch (SecurityTokenExpiredException)
            {
                return NotFound(new ErrorResponse("Unkown or expired refresh token"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [Authorize]
        [HttpGet, Route("account")]
        public async Task<IActionResult> GetAccount()
        {
            var id = HttpContext.User.FindFirstValue(ClaimTypes.Uri);
            if (id == null)
            {
                return BadRequest("Invalid token");
            }
            try
            {
                return Ok(await _accountService.GetAccount(id));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
