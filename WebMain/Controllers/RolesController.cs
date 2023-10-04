using AccountBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Common.Models;
using Common.Enums;

namespace Web.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;
        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }
        [HttpPost, Route("add-student")]
        public async Task<IActionResult> AddStudentRole([FromQuery]Guid GroupId)
        {
            var id = HttpContext.User.FindFirstValue(ClaimTypes.Uri);
            if (id == null)
            {
                return BadRequest("Invalid token");
            }
            try
            {
                await _rolesService.AddStudentRole(id, GroupId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost, Route("add-professor")]
        public async Task<IActionResult> AddProfessorRole([FromQuery] Guid ProfessorId)
        {
            var id = HttpContext.User.FindFirstValue(ClaimTypes.Uri);
            if (id == null)
            {
                return BadRequest("Invalid token");
            }
            try
            {
                await _rolesService.AddStudentRole(id, ProfessorId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpPost, Route("add-editor")]
        public async Task<IActionResult> AddEditorRole()
        {
            var id = HttpContext.User.FindFirstValue(ClaimTypes.Uri);
            if (id == null)
            {
                return BadRequest("Invalid token");
            }
            try
            {
                await _rolesService.AddEditorRole(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveRole([FromQuery] Roles Role)
        {
            var id = HttpContext.User.FindFirstValue(ClaimTypes.Uri);
            if (id == null)
            {
                return BadRequest("Invalid token");
            }
            try
            {
                await _rolesService.RemoveRole(id, Role);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ex));
            }
        }
    }
}
