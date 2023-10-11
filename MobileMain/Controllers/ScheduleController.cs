using Common.Enums;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;

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
        public IActionResult GetSchedule([FromRoute] ScheduleFilter type, [FromQuery] DateTime day, Guid filterItemId, bool getWeek = true)
        {
            try
            {
                if (getWeek)
                {
                    return Ok(_scheduleProviderService.GetWeekSchedule(day, filterItemId, type));
                }
                else
                {
                    return Ok(_scheduleProviderService.GetDaySchedule(day, filterItemId, type));
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
    }
}
