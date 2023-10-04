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
    [Route("api/professors")]
    [ApiController]
    public class ProfessorsController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        private readonly IProfessorsService _professorsService;
        public ProfessorsController(IItemsListService itemsListService, IProfessorsService professorsService)
        {
            _itemsListService = itemsListService;
            _professorsService = professorsService;
        }
        [HttpGet]
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
        [HttpPost]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> CreateProfessor(ProfessorModel model)
        {
            try
            {
                var DTO = await _professorsService.CreateProfessor(model);
                return Created(DTO.Id.ToString(), DTO);
            }
            catch (DataConflictException)
            {
                return BadRequest(new ErrorResponse("The given name conflicts with existing professor's name"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> EditProfessor([FromRoute] Guid id, [FromBody] ProfessorModel model)
        {
            try
            {
                await _professorsService.EditProfessor(id, model);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the professor with given id"));
            }
            catch (DataConflictException)
            {
                return BadRequest(new ErrorResponse("The given name conflicts with existing professor's name"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> DeleteProfessor([FromRoute] Guid id)
        {
            try
            {
                await _professorsService.DeleteProfessor(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the professor with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
