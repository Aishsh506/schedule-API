using Common.DTO;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleBL;
using ScheduleBL.DTO;
using System.ComponentModel.DataAnnotations;
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
        [HttpGet, Route("times")]
        [ProducesResponseType(typeof(LessonTimesDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public ActionResult<LessonTimesDTO> GetLessonTimes([FromQuery][Range(1, 7)] uint timeslot)
        {
            try
            {
                return Ok(new LessonTimesDTO(timeslot));
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest(new ErrorResponse("Invalid timeslot value. Must be between 1 and 7"));
            }
        }
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(LessonDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
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
        [ProducesResponseType(typeof(DayDTO[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetSchedule([FromRoute] ScheduleFilter type, [FromQuery] DateTime day, Guid filterItemId, bool getWeek = true, bool withBreaks = false)
        {
            try
            {
                if (getWeek)
                {
                    return Ok(_scheduleProviderService.GetWeekSchedule(day, filterItemId, type, withBreaks));
                }
                else
                {
                    return Ok(_scheduleProviderService.GetDaySchedule(day, filterItemId, type, withBreaks));
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
        [ProducesResponseType(typeof(IdDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(OverlayResponse), 409)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CreateLesson(LessonModel model)
        {
            try
            {
                var DTO = await _scheduleEditorService.CreateLesson(model);
                return Created(DTO.Id.ToString(), DTO);
            }
            catch (DataConflictException ex)
            {
                return Conflict(new OverlayResponse(ex));
            }
            catch (ArgumentException)
            {
                return BadRequest(new ErrorResponse("Invalid startDate/endDate/dayOfWeek"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorResponse($"Could not find the {ex.Message} with given id"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPatch, Route("{id}")]
        [Authorize(Roles = "Editor")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(OverlayResponse), 409)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> EditLesson([FromRoute] Guid id, [FromBody] LessonEditModel model)
        {
            try
            {
                await _scheduleEditorService.EditLesson(id, model);
                return Ok();
            }
            catch (DataConflictException ex)
            {
                return Conflict(new OverlayResponse(ex));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound($"Could not find the {ex.Message} with given id");
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
