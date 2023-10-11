using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;
using Web.Models;
using WebBL.Models;

namespace Web.Controllers
{
    [Route("lessons")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleEditorService _scheduleEditorService;
        private readonly IScheduleProviderService _scheduleProviderService;
        public ScheduleController(IScheduleEditorService scheduleEditorService, IScheduleProviderService scheduleProviderService)
        {
            _scheduleEditorService = scheduleEditorService;
            _scheduleProviderService = scheduleProviderService;
        }
        [HttpGet, Route("{id}")]
        public IActionResult GetLesson([FromRoute] Guid id)
        {
            try
            {
                return Ok(_scheduleEditorService.GetLesson(id));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the lesson with given id"));
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpGet, Route("search/{type}")]
        public IActionResult GetSchedule([FromRoute] ScheduleFilter type, [FromQuery] DateTime day, Guid filterItemId, bool getWeek = true)
        {
            try
            {
                if (getWeek)
                {
                    return Ok(_scheduleProviderService.GetWeekSchedule(day, filterItemId, type));
                }
                else
                {
                    return Ok(_scheduleProviderService.GetDaySchedule(day, filterItemId, type));
                }
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse($"Could not find the {ex.Message} with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> CreateLesson(LessonModel model)
        {
            try
            {
                var DTO = await _scheduleEditorService.CreateLesson(model);
                return Created(DTO.Id.ToString(), DTO);
            }
            catch (DataConflictException ex)
            {
                return BadRequest(new OverlayResponse(ex));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"Could not found the {ex.Message} with given id");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPatch, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> EditLesson([FromRoute] Guid id, [FromBody] LessonEditModel model)
        {
            try
            {
                await _scheduleEditorService.EditLesson(id, model);
                return Ok();
            }
            catch (DataConflictException ex)
            {
                return BadRequest(new OverlayResponse(ex));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"Could not found the {ex.Message} with given id");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> DeleteLesson([FromRoute] Guid id)
        {
            try
            {
                await _scheduleEditorService.DeleteLesson(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponse("Could not find the lesson with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
