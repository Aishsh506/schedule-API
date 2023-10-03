using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;

namespace MobileMain.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        public SubjectsController(IItemsListService itemsListService)
        {
            _itemsListService = itemsListService;
        }
        [HttpGet]
        public IActionResult GetSubjects()
        {
            try
            {
                return Ok(_itemsListService.GetSubjects());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
