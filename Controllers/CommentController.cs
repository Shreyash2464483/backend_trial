using backend_trial.Data;
using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
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
        private readonly IdeaBoardDbContext _dbContext;

        public CommentController(IdeaBoardDbContext dbContext)
        {
            _dbContext = dbContext;
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

                // Verify idea exists
                var idea = await _dbContext.Ideas.FirstOrDefaultAsync(i => i.IdeaId == ideaId);
                if (idea == null)
                {
                    return NotFound(new { Message = "Idea not found" });
                }

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userGuid);
                if (user == null)
                {
                    return Unauthorized(new { Message = "User not found" });
                }

                var comment = new Comment
                {
                    CommentId = Guid.NewGuid(),
                    IdeaId = ideaId,
                    UserId = userGuid,
                    Text = request.Text,
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.Comments.Add(comment);
                await _dbContext.SaveChangesAsync();

                var response = new CommentResponseDto
                {
                    CommentId = comment.CommentId,
                    UserId = comment.UserId,
                    UserName = user.Name,
                    Text = comment.Text,
                    CreatedDate = comment.CreatedDate
                };

                return CreatedAtAction(nameof(GetCommentById), new { id = comment.CommentId }, response);
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
                var idea = await _dbContext.Ideas.FirstOrDefaultAsync(i => i.IdeaId == ideaId);
                if (idea == null)
                {
                    return NotFound(new { Message = "Idea not found" });
                }

                var comments = await _dbContext.Comments
                    .Where(c => c.IdeaId == ideaId)
                    .Include(c => c.User)
                    .OrderByDescending(c => c.CreatedDate)
                    .Select(c => new CommentResponseDto
                    {
                        CommentId = c.CommentId,
                        UserId = c.UserId,
                        UserName = c.User.Name,
                        Text = c.Text,
                        CreatedDate = c.CreatedDate
                    })
                    .ToListAsync();

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
                var comment = await _dbContext.Comments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CommentId == id);

                if (comment == null)
                {
                    return NotFound(new { Message = "Comment not found" });
                }

                var response = new CommentResponseDto
                {
                    CommentId = comment.CommentId,
                    UserId = comment.UserId,
                    UserName = comment.User.Name,
                    Text = comment.Text,
                    CreatedDate = comment.CreatedDate
                };

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

                var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
                if (comment == null)
                {
                    return NotFound(new { Message = "Comment not found" });
                }

                // Check if user is the owner of the comment
                if (comment.UserId != userGuid)
                {
                    return Forbid("You can only update your own comments");
                }

                comment.Text = request.Text;
                _dbContext.Comments.Update(comment);
                await _dbContext.SaveChangesAsync();

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userGuid);
                var response = new CommentResponseDto
                {
                    CommentId = comment.CommentId,
                    UserId = comment.UserId,
                    UserName = user?.Name ?? "Unknown",
                    Text = comment.Text,
                    CreatedDate = comment.CreatedDate
                };

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

                var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
                if (comment == null)
                {
                    return NotFound(new { Message = "Comment not found" });
                }

                // Check if user is the owner of the comment
                if (comment.UserId != userGuid)
                {
                    return Forbid("You can only delete your own comments");
                }

                _dbContext.Comments.Remove(comment);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Message = "Comment deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting comment", Error = ex.Message });
            }
        }
    }
}
