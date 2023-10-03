using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;

namespace MobileMain.Controllers
{
    [Route("api/buildings")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        private readonly IScheduleItemsService _scheduleItemsService;
        public BuildingsController(IScheduleItemsService scheduleItemsService)
        {
            _scheduleItemsService = scheduleItemsService;
        }
        [HttpGet]
        public IActionResult GetBuildings()
        {
            try
            {
                return Ok(_scheduleItemsService.GetBuildings());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
