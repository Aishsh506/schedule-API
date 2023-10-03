using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;

namespace MobileMain.Controllers
{
    [Route("api/audiences")]
    [ApiController]
    public class AudiencesController : ControllerBase
    {
        private readonly IScheduleItemsService _scheduleItemsService;
        public AudiencesController(IScheduleItemsService scheduleItemsService)
        {
            _scheduleItemsService = scheduleItemsService;
        }
        [HttpGet]
        public IActionResult GetAudiences([FromQuery] Guid BuildingId)
        {
            try
            {
                return Ok(_scheduleItemsService.GetBuildingAudiences(BuildingId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not found building with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
