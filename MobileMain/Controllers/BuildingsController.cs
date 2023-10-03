using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;

namespace MobileMain.Controllers
{
    [Route("api/buildings")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        public BuildingsController(IItemsListService itemsListService)
        {
            _itemsListService = itemsListService;
        }
        [HttpGet]
        public IActionResult GetBuildings()
        {
            try
            {
                return Ok(_itemsListService.GetBuildings());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
