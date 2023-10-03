using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;

namespace MobileMain.Controllers
{
    [Route("api/professors")]
    [ApiController]
    public class ProfessorsController : ControllerBase
    {
        private readonly IScheduleItemsService _scheduleItemsService;
        public ProfessorsController(IScheduleItemsService scheduleItemsService)
        {
            _scheduleItemsService = scheduleItemsService;
        }
        [HttpGet]
        public IActionResult GetProfessors()
        {
            try
            {
                return Ok(_scheduleItemsService.GetProfessors());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
