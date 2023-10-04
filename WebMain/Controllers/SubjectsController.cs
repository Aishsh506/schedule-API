using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;
using WebBL;
using WebBL.Models;

namespace MobileMain.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly IItemsListService _itemsListService;
        private readonly ISubjectsService _subjectsService;
        public SubjectsController(IItemsListService itemsListService, ISubjectsService subjectsService)
        {
            _itemsListService = itemsListService;
            _subjectsService = subjectsService;
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
        [HttpPost]
        public async Task<IActionResult> CreateSubject(SubjectModel model)
        {
            try
            {
                var DTO = await _subjectsService.CreateSubject(model);
                return Created(DTO.Id.ToString(), DTO);
            }
            catch (DataConflictException)
            {
                return BadRequest(new ErrorResponse("The given name conflicts with existing subject name"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost, Route("{id}")]
        public async Task<IActionResult> EditSubject([FromRoute] Guid id, [FromBody] SubjectModel model)
        {
            try
            {
                await _subjectsService.EditSubject(id, model);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the subject with given id"));
            }
            catch (DataConflictException)
            {
                return BadRequest(new ErrorResponse("The given name conflicts with existing subject name"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> DeleteSubject([FromRoute] Guid id)
        {
            try
            {
                await _subjectsService.DeleteSubject(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the subject with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
