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
    [Route("api/buildings")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        private readonly IBuildingsService _buildingsService;
        public BuildingsController(IItemsListService itemsListService, IBuildingsService buildingsService)
        {
            _itemsListService = itemsListService;
            _buildingsService = buildingsService;
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
        [HttpPost]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> CreateBuilding(BuildingModel model)
        {
            try
            {
                var DTO = await _buildingsService.CreateBuilding(model);
                return Created(DTO.Id.ToString(), DTO);
            }
            catch (DataConflictException)
            {
                return BadRequest(new ErrorResponse("The given name conflicts with existing building name"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> EditBuilding([FromRoute]Guid id, [FromBody]BuildingModel model)
        {
            try
            {
                await _buildingsService.EditBuilding(id, model);
                return Ok();
            }
            catch(KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the building with given id"));
            }
            catch(DataConflictException)
            {
                return BadRequest(new ErrorResponse("The given name conflicts with existing building name"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> DeleteBuilding([FromRoute]Guid id)
        {
            try
            {
                await _buildingsService.DeleteBuilding(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the building with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
