using Common.DTO;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;

namespace MobileMain.Controllers
{
    [Route("api/professors")]
    [ApiController]
    public class ProfessorsController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        public ProfessorsController(IItemsListService itemsListService)
        {
            _itemsListService = itemsListService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<ProfessorDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetProfessors()
        {
            try
            {
                return Ok(_itemsListService.GetProfessors());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
