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
    [Authorize(Roles = "Employee")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        // Add comment to an idea
        [HttpPost("{ideaId}")]
        public async Task<ActionResult> AddComment(Guid ideaId, [FromBody] CommentRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(request.Text))
                {
                    return BadRequest(new { Message = "Comment text cannot be empty" });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                var response = await commentRepository.AddCommentAsync(ideaId, userGuid, request);

                return CreatedAtAction(nameof(GetCommentById), new { id = response.CommentId }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding comment", Error = ex.Message });
            }
        }

        // Get all comments for an idea
        [HttpGet("{ideaId}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCommentsForIdea(Guid ideaId)
        {
            try
            {
                var comments = await commentRepository.GetCommentsForIdeaAsync(ideaId);

                return Ok(comments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving comments", Error = ex.Message });
            }
        }

        // Get comment by ID
        [HttpGet("comment/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCommentById(Guid id)
        {
            try
            {
                var response = await commentRepository.GetCommentByIdAsync(id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving comment", Error = ex.Message });
            }
        }

        // Update comment (only own comments)
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateComment(Guid id, [FromBody] CommentRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrWhiteSpace(request.Text))
                {
                    return BadRequest(new { Message = "Comment text cannot be empty" });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                var response = await commentRepository.UpdateCommentAsync(id, userGuid, request);

                return Ok(new { Message = "Comment updated successfully", Comment = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating comment", Error = ex.Message });
            }
        }

        // Delete comment (only own comments)
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                await commentRepository.DeleteCommentAsync(id, userGuid);

                return Ok(new { Message = "Comment deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting comment", Error = ex.Message });
            }
        }
    }
}
