using Common.DTO;
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
        [HttpPost]
        [Authorize(Roles = "Editor")]
        [ProducesResponseType(typeof(IdDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CreateAudience(AudienceModel model)
        {
            try
            {
                var DTO = await _audiencesService.CreateAudience(model);
                return Created(DTO.Id.ToString(), DTO);
            }
            catch (DataConflictException)
            {
                return Conflict(new ErrorResponse("An audience with the given name has already been created for the building"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPut, Route("{id}")]
        [Authorize(Roles = "Editor")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
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
                return Conflict(new ErrorResponse("An audience with the given name has already been created for the building"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = "Editor")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
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
