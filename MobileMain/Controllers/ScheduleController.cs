using Common.Enums;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;
using ScheduleBL.DTO;
using System.ComponentModel.DataAnnotations;
using Common.Dictionaries;
using Common.DTO;

namespace Web.Controllers
{
    [Route("lessons")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleProviderService _scheduleProviderService;
        public ScheduleController(IScheduleProviderService scheduleProviderService)
        {
            _scheduleProviderService = scheduleProviderService;
        }
        [HttpGet, Route("search/{type}")]
        [ProducesResponseType(typeof(DayDTO[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetSchedule([FromRoute] ScheduleFilter type, [FromQuery] DateTime day, Guid filterItemId, bool getWeek = true, bool withBreaks = false)
        {
            try
            {
                if (getWeek)
                {
                    return Ok(_scheduleProviderService.GetWeekSchedule(day, filterItemId, type, withBreaks));
                }
                else
                {
                    return Ok(_scheduleProviderService.GetDaySchedule(day, filterItemId, type, withBreaks));
                }
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse($"Could not find the {ex.Message} with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpGet, Route("times")]
        [ProducesResponseType(typeof(LessonTimesDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public ActionResult<LessonTimesDTO> GetLessonTimes([FromQuery][Range(1, 7)] uint timeslot)
        {
            try
            {
                return Ok(new LessonTimesDTO(timeslot));
            }
            catch(ArgumentOutOfRangeException)
            {
                return BadRequest(new ErrorResponse("Invalid timeslot value. Must be between 1 and 7"));
            }
        }
    }
}
