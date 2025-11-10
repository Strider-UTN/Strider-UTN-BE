using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriderWebApi.Dto.Groups;
using StriderWebApi.Dto.Invitation;
using StriderWebApi.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StriderWebApi.Controllers
{
    /// <summary>
    /// Controller para gestionar sedes/grupos de entrenamiento
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrainingGroupsController(ITrainingGroupService trainingGroupService) : ControllerBase
    {

        /// <summary>
        /// Obtiene todas las sedes del coach actual
        /// GET /api/TrainingGroups
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainingGroupResponseDto>>> GetMyTrainingGroups()
        {
            try
            {
                var groups = await trainingGroupService.GetMyTrainingGroupsAsync();
                return Ok(groups);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las sedes", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una sede por su ID
        /// GET /api/TrainingGroups/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingGroupResponseDto>> GetTrainingGroupById(int id)
        {
            try
            {
                var group = await trainingGroupService.GetTrainingGroupByIdAsync(id);
                if (group == null)
                {
                    return NotFound(new { message = $"Sede con ID {id} no encontrada" });
                }

                return Ok(group);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la sede", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva sede
        /// POST /api/TrainingGroups
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TrainingGroupResponseDto>> CreateTrainingGroup([FromBody] CreateTrainingGroupDto dto)
        {
            try
            {
                var createdGroup = await trainingGroupService.CreateTrainingGroupAsync(dto);
                return CreatedAtAction(nameof(GetTrainingGroupById), new { id = createdGroup.Id }, createdGroup);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la sede", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una sede existente
        /// PUT /api/TrainingGroups/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TrainingGroupResponseDto>> UpdateTrainingGroup(int id, [FromBody] UpdateTrainingGroupDto dto)
        {
            try
            {
                var updatedGroup = await trainingGroupService.UpdateTrainingGroupAsync(id, dto);
                return Ok(updatedGroup);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la sede", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una sede
        /// DELETE /api/TrainingGroups/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainingGroup(int id)
        {
            try
            {
                await trainingGroupService.DeleteTrainingGroupAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la sede", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los miembros de una sede
        /// GET /api/TrainingGroups/{id}/members
        /// </summary>
        [HttpGet("{id}/members")]
        public async Task<ActionResult<IEnumerable<TrainingGroupMemberResponseDto>>> GetGroupMembers(int id)
        {
            try
            {
                var members = await trainingGroupService.GetGroupMembersAsync(id);
                return Ok(members);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los miembros", error = ex.Message });
            }
        }

        // TODO: Endpoints de invitaciones se implementarán más adelante
        // POST /api/TrainingGroups/{id}/invite
        // DELETE /api/TrainingGroups/{groupId}/members/{memberId}

        /// <summary>
        /// Obtiene las estadísticas de una sede
        /// GET /api/TrainingGroups/{id}/stats
        /// </summary>
        [HttpGet("{id}/stats")]
        public async Task<ActionResult<TrainingGroupStatsDto>> GetGroupStats(int id)
        {
            try
            {
                var stats = await trainingGroupService.GetGroupStatsAsync(id);
                return Ok(stats);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las estadísticas", error = ex.Message });
            }
        }

        /// <summary>
        /// Invita a un atleta a una sede
        /// POST /api/TrainingGroups/{groupId}/invite
        /// </summary>
        [HttpPost("{groupId}/invite")]
        public async Task<ActionResult<GroupInvitationResponseDto>> InviteAthleteToGroup(
            int groupId,
            [FromBody] InviteGroupMemberDto dto)
        {
            try
            {
                var invitation = await trainingGroupService.InviteAthleteToGroupAsync(groupId, dto);
                return Ok(invitation);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al enviar la invitación", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las invitaciones pendientes del atleta actual
        /// GET /api/TrainingGroups/my-invitations
        /// </summary>
        [HttpGet("my-invitations")]
        public async Task<ActionResult<IEnumerable<GroupInvitationResponseDto>>> GetMyPendingInvitations()
        {
            try
            {
                var invitations = await trainingGroupService.GetMyPendingInvitationsAsync();
                return Ok(invitations);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las invitaciones", error = ex.Message });
            }
        }

        /// <summary>
        /// Responde a una invitación de sede
        /// POST /api/TrainingGroups/invitations/{invitationId}/respond
        /// </summary>
        [HttpPost("invitations/{invitationId}/respond")]
        public async Task<ActionResult<GroupInvitationResponseDto>> RespondToInvitation(
            int invitationId,
            [FromBody] RespondToGroupInvitationDto dto)
        {
            try
            {
                var invitation = await trainingGroupService.RespondToInvitationAsync(invitationId, dto);
                return Ok(invitation);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al responder la invitación", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las sedes activas del atleta actual
        /// GET /api/TrainingGroups/my-groups
        /// </summary>
        [HttpGet("my-groups")]
        public async Task<ActionResult<IEnumerable<MyTrainingGroupResponseDto>>> GetMyGroups()
        {
            try
            {
                var groups = await trainingGroupService.GetMyGroupsAsync();
                return Ok(groups);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las sedes", error = ex.Message });
            }
        }

        /// <summary>
        /// El atleta abandona una sede
        /// DELETE /api/TrainingGroups/{groupId}/leave
        /// </summary>
        [HttpDelete("{groupId}/leave")]
        public async Task<IActionResult> LeaveGroup(int groupId)
        {
            try
            {
                await trainingGroupService.LeaveGroupAsync(groupId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al abandonar la sede", error = ex.Message });
            }
        }

        /// <summary>
        /// El coach elimina un miembro de una sede
        /// DELETE /api/TrainingGroups/{groupId}/members/{memberId}
        /// </summary>
        [HttpDelete("{groupId}/members/{memberId}")]
        public async Task<IActionResult> RemoveMemberFromGroup(int groupId, int memberId)
        {
            try
            {
                await trainingGroupService.RemoveMemberFromGroupAsync(groupId, memberId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el miembro", error = ex.Message });
            }
        }
    }
}
