using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;

namespace MobileMain.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        public GroupsController(IItemsListService itemsListService)
        {
            _itemsListService = itemsListService;
        }
        [HttpGet]
        public IActionResult GetGroups()
        {
            try
            {
                return Ok(_itemsListService.GetGroups());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
