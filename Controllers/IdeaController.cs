using backend_trial.Data;
using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
using backend_trial.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_trial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdeaController : ControllerBase
    {
        private readonly IIdeaRepository ideaRepository;

        public IdeaController(IIdeaRepository ideaRepository)
        {
            this.ideaRepository = ideaRepository;
        }

        [HttpGet("all")]
        public async Task<ActionResult> GetAllIdeas()
        {
            try
            {
                var ideas = await ideaRepository.GetAllIdeasAsync();
                return Ok(ideas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving ideas", Error = ex.Message });
            }
        }

        // Get all ideas submitted by current employee
        [HttpGet("my-ideas")]
        public async Task<ActionResult> GetMyIdeas()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                var ideas = await ideaRepository.GetMyIdeasAsync(userGuid);
                return Ok(ideas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving your ideas", Error = ex.Message });
            }
        }

        // Get idea by ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetIdeaById(Guid id)
        {
            try
            {
                var idea = await ideaRepository.GetIdeaByIdAsync(id);
                return Ok(idea);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving idea", Error = ex.Message });
            }
        }
        // Submit new idea (Employee role)
        [HttpPost("submit")]
        public async Task<ActionResult> SubmitIdea([FromBody] IdeaRequestDto ideaRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                var responseIdea = await ideaRepository.SubmitIdeaAsync(ideaRequestDto, userGuid);
                return CreatedAtAction(nameof(GetIdeaById), new { id = responseIdea.IdeaId }, responseIdea);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error submitting idea", Error = ex.Message });
            }
        }

        // Update idea (Employee can only update their own ideas)
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateIdea(Guid id, [FromBody] IdeaRequestDto ideaRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new { Message = "User ID not found in token" });
                }
                var responseIdea = await ideaRepository.UpdateIdeaAsync(id, ideaRequestDto, userGuid);
                return Ok(new { Message = "Idea updated successfully", Idea = responseIdea });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating idea", Error = ex.Message });
            }
        }

        // Delete idea (Employee can only delete their own ideas in Draft status)
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIdea(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new { Message = "User ID not found in token" });
                }
                await ideaRepository.DeleteIdeaAsync(id, userGuid);

                return Ok(new { Message = "Idea deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting idea", Error = ex.Message });
            }
        }
    }
}

