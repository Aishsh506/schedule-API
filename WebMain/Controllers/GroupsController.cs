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
    [Route("api/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        private readonly IGroupsService _groupsService;
        public GroupsController(IItemsListService itemsListService, IGroupsService groupsService)
        {
            _itemsListService = itemsListService;
            _groupsService = groupsService;
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
        [HttpPost]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> CreateGroup(GroupModel model)
        {
            try
            {
                var DTO = await _groupsService.CreateGroup(model);
                return Created(DTO.Id.ToString(), DTO);
            }
            catch (DataConflictException)
            {
                return BadRequest(new ErrorResponse("The given name conflicts with existing group name"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPut, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> EditGroup([FromRoute] Guid id, [FromBody] GroupModel model)
        {
            try
            {
                await _groupsService.EditGroup(id, model);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the group with given id"));
            }
            catch (DataConflictException)
            {
                return BadRequest(new ErrorResponse("The given name conflicts with existing group name"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid id)
        {
            try
            {
                await _groupsService.DeleteGroup(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the group with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
