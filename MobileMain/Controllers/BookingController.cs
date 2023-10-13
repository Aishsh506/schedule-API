using Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;
using ScheduleBL.Models;
using System.Security.Claims;
using Common.Models;
using Common.DTO;
using ScheduleBL.DTO;
using System.ComponentModel.DataAnnotations;

namespace Mobile.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpGet, Route("available-audiences")]
        [ProducesResponseType(typeof(AudienceDTO[]), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetAvailableAudiences([FromQuery] DateTime date, [Range(1, 7)] uint timeslot)
        {
            try
            {
                return Ok(_bookingService.GetAvailableAudiences(date, timeslot));
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest(new ErrorResponse("Invalid timeslot value. Must be between 1 and 7"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(IdDTO), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(OverlayResponse), 409)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CreateRequest(BookingModel model)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.Uri);
            if (userId == null)
            {
                return BadRequest(new ErrorResponse("Invalid token"));
            }
            try
            {
                var dto = await _bookingService.CreateRequest(Guid.Parse(userId), model);
                return Created(dto.Id.ToString(), dto);
            }
            catch (DataConflictException ex)
            {
                if (ex.Cause == null)
                {
                    return Conflict(new ErrorResponse("The audience is occupied"));
                }
                return BadRequest(new OverlayResponse(ex));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse($"Could not find the {ex.Message} with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPatch, Route("{id}/cancel")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CancelRequest([FromRoute] Guid id)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.Uri);
            if (userId == null)
            {
                return BadRequest(new ErrorResponse("Invalid token"));
            }
            try
            {
                await _bookingService.CancelRequest(Guid.Parse(userId), id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the request with given id"));
            }
            catch (AccessDeniedException)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpGet, Route("user-requests")]
        [Authorize]
        [ProducesResponseType(typeof(BookingRequestDTO[]), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetUserRequests()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.Uri);
            if (userId == null)
            {
                return BadRequest(new ErrorResponse("Invalid token"));
            }
            try
            {
                return Ok(_bookingService.GetUserRequests(Guid.Parse(userId)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
