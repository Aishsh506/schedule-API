using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;
using WebBL;
using WebBL.Models;

namespace MobileMain.Controllers
{
    [Route("api/audiences")]
    [ApiController]
    public class AudiencesController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        private readonly IAudiencesService _audiencesService;
        public AudiencesController(IItemsListService itemsListService, IAudiencesService audiencesService)
        {
            _itemsListService = itemsListService;
            _audiencesService = audiencesService;
        }
        [HttpGet]
        public IActionResult GetAudiences([FromQuery] Guid BuildingId)
        {
            try
            {
                return Ok(_itemsListService.GetBuildingAudiences(BuildingId));
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
        [HttpPost]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> CreateAudience(AudienceModel model)
        {
            try
            {
                var DTO = await _audiencesService.CreateAudience(model);
                return Created(DTO.Id.ToString(), DTO);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the building with given id"));
            }
            catch (DataConflictException)
            {
                return BadRequest(new ErrorResponse("An audience with the given name has already been created for the building"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> EditAudience([FromRoute] Guid id, [FromBody] AudienceEditModel model)
        {
            try
            {
                await _audiencesService.EditAudience(id, model);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the audience with given id"));
            }
            catch (DataConflictException)
            {
                return BadRequest(new ErrorResponse("An audience with the given name has already been created for the building"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> DeleteAudience([FromRoute] Guid id)
        {
            try
            {
                await _audiencesService.DeleteAudience(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the audience with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
