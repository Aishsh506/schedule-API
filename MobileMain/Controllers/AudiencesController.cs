using Common.DTO;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;

namespace MobileMain.Controllers
{
    [Route("api/audiences")]
    [ApiController]
    public class AudiencesController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        public AudiencesController(IItemsListService itemsListService)
        {
            _itemsListService = itemsListService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<AudienceDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetAudiences()
        {
            try
            {
                return Ok(_itemsListService.GetAudiences());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
