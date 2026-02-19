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
    public class VoteController : ControllerBase
    {
        private readonly IdeaBoardDbContext _dbContext;

        public VoteController(IdeaBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get current user's vote status for an idea
        [HttpGet("{ideaId}/user-vote")]
        [Authorize]
        public async Task<ActionResult<UserVoteResponseDto>> GetUserVoteStatus(Guid ideaId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                // Check if idea exists
                var idea = await _dbContext.Ideas.FirstOrDefaultAsync(i => i.IdeaId == ideaId);
                if (idea == null)
                {
                    return NotFound(new { Message = "Idea not found" });
                }

                // Get user's vote for this idea
                var userVote = await _dbContext.Votes
                    .FirstOrDefaultAsync(v => v.IdeaId == ideaId && v.UserId == userGuid);

                var response = new UserVoteResponseDto
                {
                    HasVoted = userVote != null,
                    VoteType = userVote?.VoteType.ToString()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving user vote status", Error = ex.Message });
            }
        }

        // Add upvote to an idea
        [HttpPost("{ideaId}/upvote")]
        public async Task<ActionResult> AddUpvote(Guid ideaId)
        {
            try
            {
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

                // Check if user already voted on this idea
                var existingVote = await _dbContext.Votes
                    .FirstOrDefaultAsync(v => v.IdeaId == ideaId && v.UserId == userGuid);

                if (existingVote != null)
                {
                    // If user already upvoted, return error
                    if (existingVote.VoteType == VoteType.Upvote)
                    {
                        return BadRequest(new { Message = "You have already upvoted this idea" });
                    }

                    // If user downvoted before, remove the downvote comment and update vote
                    var downvoteComment = await _dbContext.Comments
                        .Where(c => c.IdeaId == ideaId && c.UserId == userGuid)
                        .OrderByDescending(c => c.CreatedDate)
                        .FirstOrDefaultAsync();

                    if (downvoteComment != null)
                    {
                        _dbContext.Comments.Remove(downvoteComment);
                    }

                    existingVote.VoteType = VoteType.Upvote;
                    _dbContext.Votes.Update(existingVote);
                }
                else
                {
                    // Create new upvote
                    var vote = new Vote
                    {
                        VoteId = Guid.NewGuid(),
                        IdeaId = ideaId,
                        UserId = userGuid,
                        VoteType = VoteType.Upvote
                    };

                    _dbContext.Votes.Add(vote);
                }

                await _dbContext.SaveChangesAsync();

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userGuid);
                var vote_ = await _dbContext.Votes
                    .FirstOrDefaultAsync(v => v.IdeaId == ideaId && v.UserId == userGuid);

                var response = new VoteResponseDto
                {
                    VoteId = vote_.VoteId,
                    IdeaId = vote_.IdeaId,
                    UserId = vote_.UserId,
                    UserName = user?.Name ?? "Unknown",
                    VoteType = vote_.VoteType.ToString()
                };

                return Ok(new { Message = "Upvote added successfully", Vote = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding upvote", Error = ex.Message });
            }
        }

        // Add downvote to an idea with mandatory comment
        [HttpPost("{ideaId}/downvote")]
        public async Task<ActionResult> AddDownvote(Guid ideaId, [FromBody] VoteWithCommentRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate comment is provided for downvote
                if (string.IsNullOrWhiteSpace(request.CommentText))
                {
                    return BadRequest(new { Message = "Comment is mandatory when downvoting. Please provide a reason for your downvote." });
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

                // Check if user already voted on this idea
                var existingVote = await _dbContext.Votes
                    .FirstOrDefaultAsync(v => v.IdeaId == ideaId && v.UserId == userGuid);

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userGuid);
                if (user == null)
                {
                    return Unauthorized(new { Message = "User not found" });
                }

                if (existingVote != null)
                {
                    // If user already downvoted, return error
                    if (existingVote.VoteType == VoteType.Downvote)
                    {
                        return BadRequest(new { Message = "You have already downvoted this idea" });
                    }

                    // If user upvoted before, remove upvote and add downvote with comment
                    existingVote.VoteType = VoteType.Downvote;
                    _dbContext.Votes.Update(existingVote);
                }
                else
                {
                    // Create new downvote
                    var vote = new Vote
                    {
                        VoteId = Guid.NewGuid(),
                        IdeaId = ideaId,
                        UserId = userGuid,
                        VoteType = VoteType.Downvote
                    };

                    _dbContext.Votes.Add(vote);
                }

                // Add mandatory comment for downvote
                var comment = new Comment
                {
                    CommentId = Guid.NewGuid(),
                    IdeaId = ideaId,
                    UserId = userGuid,
                    Text = request.CommentText,
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.Comments.Add(comment);
                await _dbContext.SaveChangesAsync();

                var vote_ = await _dbContext.Votes
                    .FirstOrDefaultAsync(v => v.IdeaId == ideaId && v.UserId == userGuid);

                var response = new VoteResponseDto
                {
                    VoteId = vote_.VoteId,
                    IdeaId = vote_.IdeaId,
                    UserId = vote_.UserId,
                    UserName = user.Name,
                    VoteType = vote_.VoteType.ToString()
                };

                return Ok(new { Message = "Downvote added successfully with comment", Vote = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding downvote", Error = ex.Message });
            }
        }

        // Remove vote (can upvote/downvote again after removing)
        [HttpDelete("{ideaId}")]
        public async Task<ActionResult> RemoveVote(Guid ideaId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(new { Message = "User ID not found in token" });
                }

                // Find the vote
                var vote = await _dbContext.Votes
                    .FirstOrDefaultAsync(v => v.IdeaId == ideaId && v.UserId == userGuid);

                if (vote == null)
                {
                    return NotFound(new { Message = "Vote not found" });
                }

                // If it was a downvote, remove the associated comment
                if (vote.VoteType == VoteType.Downvote)
                {
                    var comment = await _dbContext.Comments
                        .Where(c => c.IdeaId == ideaId && c.UserId == userGuid)
                        .OrderByDescending(c => c.CreatedDate)
                        .FirstOrDefaultAsync();

                    if (comment != null)
                    {
                        _dbContext.Comments.Remove(comment);
                    }
                }

                _dbContext.Votes.Remove(vote);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Message = "Vote removed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error removing vote", Error = ex.Message });
            }
        }

        // Get all votes for an idea
        [HttpGet("{ideaId}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetVotesForIdea(Guid ideaId)
        {
            try
            {
                var votes = await _dbContext.Votes
                    .Where(v => v.IdeaId == ideaId)
                    .Include(v => v.User)
                    .Select(v => new VoteResponseDto
                    {
                        VoteId = v.VoteId,
                        IdeaId = v.IdeaId,
                        UserId = v.UserId,
                        UserName = v.User.Name,
                        VoteType = v.VoteType.ToString()
                    })
                    .ToListAsync();

                return Ok(votes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving votes", Error = ex.Message });
            }
        }
    }
}

