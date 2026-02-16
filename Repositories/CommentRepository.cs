using backend_trial.Data;
using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_trial.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IdeaBoardDbContext dbContext;
        public CommentRepository(IdeaBoardDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<CommentResponseDto> AddCommentAsync(Guid ideaId, Guid userGuid, CommentRequestDto request)
        {
            // Verify idea exists
            var idea = await dbContext.Ideas.FirstOrDefaultAsync(i => i.IdeaId == ideaId);
            if (idea == null)
            {
                throw new InvalidOperationException("Idea not found!");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userGuid);
            if (user == null)
            {
                throw new InvalidOperationException("User not found!");
            }

            var comment = new Comment
            {
                CommentId = Guid.NewGuid(),
                IdeaId = ideaId,
                UserId = userGuid,
                Text = request.Text,
                CreatedDate = DateTime.UtcNow
            };

            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();

            var response = new CommentResponseDto
            {
                CommentId = comment.CommentId,
                UserId = comment.UserId,
                UserName = user.Name,
                Text = comment.Text,
                CreatedDate = comment.CreatedDate
            };
            return response;
        }

        public async Task<List<CommentResponseDto>> GetCommentsForIdeaAsync(Guid ideaId)
        {
            var idea = await dbContext.Ideas.FirstOrDefaultAsync(i => i.IdeaId == ideaId);
            if (idea == null)
            {
                throw new InvalidOperationException("Idea not found!");
            }

            var comments = await dbContext.Comments
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
            return comments;
        }

        public async Task<CommentResponseDto> GetCommentByIdAsync(Guid id)
        {
            var comment = await dbContext.Comments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
            {
                throw new InvalidOperationException("Comment not found");
            }

            var response = new CommentResponseDto
            {
                CommentId = comment.CommentId,
                UserId = comment.UserId,
                UserName = comment.User.Name,
                Text = comment.Text,
                CreatedDate = comment.CreatedDate
            };

            return response;
        }


        public async Task<CommentResponseDto> UpdateCommentAsync(Guid id, Guid userGuid, CommentRequestDto request)
        {
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
            if (comment == null)
            {
               throw new InvalidOperationException("Comment text cannot be empty");
            }

            // Check if user is the owner of the comment
            if (comment.UserId != userGuid)
            {
                throw new InvalidOperationException("You can only update your own comments");
            }

            comment.Text = request.Text;
            dbContext.Comments.Update(comment);
            await dbContext.SaveChangesAsync();

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userGuid);
            var response = new CommentResponseDto
            {
                CommentId = comment.CommentId,
                UserId = comment.UserId,
                UserName = user?.Name ?? "Unknown",
                Text = comment.Text,
                CreatedDate = comment.CreatedDate
            };
            return response;
        }

        public async Task<bool> DeleteCommentAsync(Guid id, Guid userGuid)
        {
            var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
            if (comment == null)
            {
                throw new InvalidOperationException("Comment not found");
            }

            // Check if user is the owner of the comment
            if (comment.UserId != userGuid)
            {
                throw new InvalidOperationException("You can only delete your own comments");
            }

            dbContext.Comments.Remove(comment);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
