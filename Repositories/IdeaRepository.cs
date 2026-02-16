using backend_trial.Data;
using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace backend_trial.Repositories
{
    public class IdeaRepository : IIdeaRepository
    {
        private readonly IdeaBoardDbContext dbContext;
        public IdeaRepository(IdeaBoardDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IdeaBoardDbContext DbContext { get; }

        public async Task<List<IdeaResponseDto>> GetAllIdeasAsync()
        {
            var ideas = await dbContext.Ideas
                   .Include(i => i.Category)
                   .Include(i => i.SubmittedByUser)
                   .Include(i => i.Comments)
                       .ThenInclude(c => c.User)
                   .Include(i => i.Votes)
                   .Select(i => new IdeaResponseDto
                   {
                       IdeaId = i.IdeaId,
                       Title = i.Title,
                       Description = i.Description,
                       CategoryId = i.CategoryId,
                       CategoryName = i.Category.Name,
                       SubmittedByUserId = i.SubmittedByUserId,
                       SubmittedByUserName = i.SubmittedByUser.Name,
                       SubmittedDate = i.SubmittedDate,
                       Status = i.Status.ToString(),
                       Upvotes = i.Votes.Count(v => v.VoteType == VoteType.Upvote),
                       Downvotes = i.Votes.Count(v => v.VoteType == VoteType.Downvote),
                       Comments = i.Comments.Select(c => new CommentResponseDto
                       {
                           CommentId = c.CommentId,
                           UserId = c.UserId,
                           UserName = c.User.Name,
                           Text = c.Text,
                           CreatedDate = c.CreatedDate
                       }).ToList()
                   })
                   .ToListAsync();
            return ideas;
        }
    
        public async Task<List<IdeaResponseDto>> GetMyIdeasAsync(Guid userGuid)
        {
            var ideas = await dbContext.Ideas
                    .Where(i => i.SubmittedByUserId == userGuid)
                    .Include(i => i.Category)
                    .Include(i => i.Comments)
                    .ThenInclude(c => c.User)
                    .Include(i => i.Votes)
                    .Select(i => new IdeaResponseDto
                    {
                        IdeaId = i.IdeaId,
                        Title = i.Title,
                        Description = i.Description,
                        CategoryId = i.CategoryId,
                        CategoryName = i.Category.Name,
                        SubmittedByUserId = i.SubmittedByUserId,
                        SubmittedByUserName = i.SubmittedByUser.Name,
                        SubmittedDate = i.SubmittedDate,
                        Status = i.Status.ToString(),
                        Upvotes = i.Votes.Count(v => v.VoteType == VoteType.Upvote),
                        Downvotes = i.Votes.Count(v => v.VoteType == VoteType.Downvote),
                        Comments = i.Comments.Select(c => new CommentResponseDto
                        {
                            CommentId = c.CommentId,
                            UserId = c.UserId,
                            UserName = c.User.Name,
                            Text = c.Text,
                            CreatedDate = c.CreatedDate
                        }).ToList()
                    })
                    .OrderByDescending(i => i.SubmittedDate)
                    .ToListAsync();
            return ideas;
        }

        public async Task<IdeaResponseDto> GetIdeaByIdAsync(Guid id)
        {
            var idea = await dbContext.Ideas
                    .Where(i => i.IdeaId == id)
                    .Include(i => i.Category)
                    .Include(i => i.SubmittedByUser)
                    .Include(i => i.Comments)
                        .ThenInclude(c => c.User)
                    .Include(i => i.Votes)
                    .Select(i => new IdeaResponseDto
                    {
                        IdeaId = i.IdeaId,
                        Title = i.Title,
                        Description = i.Description,
                        CategoryId = i.CategoryId,
                        CategoryName = i.Category.Name,
                        SubmittedByUserId = i.SubmittedByUserId,
                        SubmittedByUserName = i.SubmittedByUser.Name,
                        SubmittedDate = i.SubmittedDate,
                        Status = i.Status.ToString(),
                        Upvotes = i.Votes.Count(v => v.VoteType == VoteType.Upvote),
                        Downvotes = i.Votes.Count(v => v.VoteType == VoteType.Downvote),
                        Comments = i.Comments.Select(c => new CommentResponseDto
                        {
                            CommentId = c.CommentId,
                            UserId = c.UserId,
                            UserName = c.User.Name,
                            Text = c.Text,
                            CreatedDate = c.CreatedDate
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

            if (idea == null)
            {
                throw new InvalidOperationException("Idea not found");
            }
            return idea;
        }

        public async Task<IdeaResponseDto> SubmitIdeaAsync(IdeaRequestDto ideaRequestDto, Guid userGuid)
        {
            var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == ideaRequestDto.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Category not found");
            }

            if (!category.IsActive)
            {
                throw new InvalidOperationException("Selected category is inactive");
            }

            // Verify user exists
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userGuid);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var newIdea = new Idea
            {
                IdeaId = Guid.NewGuid(),
                Title = ideaRequestDto.Title,
                Description = ideaRequestDto.Description,
                CategoryId = ideaRequestDto.CategoryId,
                SubmittedByUserId = userGuid,
                SubmittedDate = DateTime.UtcNow,
                Status = IdeaStatus.UnderReview
            };

            dbContext.Ideas.Add(newIdea);
            await dbContext.SaveChangesAsync();

            var responseIdea = new IdeaResponseDto
            {
                IdeaId = newIdea.IdeaId,
                Title = newIdea.Title,
                Description = newIdea.Description,
                CategoryId = newIdea.CategoryId,
                CategoryName = category.Name,
                SubmittedByUserId = newIdea.SubmittedByUserId,
                SubmittedByUserName = user.Name,
                SubmittedDate = newIdea.SubmittedDate,
                Status = newIdea.Status.ToString(),
                Upvotes = 0,
                Downvotes = 0,
                Comments = new List<CommentResponseDto>()
            };
            return responseIdea;
        }   
        public async Task<IdeaResponseDto> UpdateIdeaAsync(Guid id, IdeaRequestDto ideaRequestDto, Guid userGuid)
        {
            var idea = await dbContext.Ideas
                    .Include(i => i.Category)
                    .Include(i => i.Votes)
                    .Include(i => i.Comments)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(i => i.IdeaId == id);

            if (idea == null)
            {
                throw new InvalidOperationException("Idea not found");
            }

            // Check if user is the owner of the idea
            if (idea.SubmittedByUserId != userGuid)
            {
                throw new InvalidOperationException("You can only update your own ideas");
            }

            // Verify category exists and is active
            var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == ideaRequestDto.CategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Category not found");
            }

            if (!category.IsActive)
            {
                throw new InvalidOperationException("Selected category is inactive");
            }

            idea.Title = ideaRequestDto.Title;
            idea.Description = ideaRequestDto.Description;
            idea.CategoryId = ideaRequestDto.CategoryId;

            dbContext.Ideas.Update(idea);
            await dbContext.SaveChangesAsync();

            var updatedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userGuid);

            var responseIdea = new IdeaResponseDto
            {
                IdeaId = idea.IdeaId,
                Title = idea.Title,
                Description = idea.Description,
                CategoryId = idea.CategoryId,
                CategoryName = category.Name,
                SubmittedByUserId = idea.SubmittedByUserId,
                SubmittedByUserName = updatedUser?.Name ?? "Unknown",
                SubmittedDate = idea.SubmittedDate,
                Status = idea.Status.ToString(),
                Upvotes = idea.Votes.Count(v => v.VoteType == VoteType.Upvote),
                Downvotes = idea.Votes.Count(v => v.VoteType == VoteType.Downvote),
                Comments = idea.Comments.Select(c => new CommentResponseDto
                {
                    CommentId = c.CommentId,
                    UserId = c.UserId,
                    UserName = c.User.Name,
                    Text = c.Text,
                    CreatedDate = c.CreatedDate
                }).ToList()
            };
            return responseIdea;
        }

        public async Task<bool> DeleteIdeaAsync(Guid id, Guid userGuid)
        {
            var idea = await dbContext.Ideas.FirstOrDefaultAsync(i => i.IdeaId == id);
            if (idea == null)
            {
                throw new InvalidOperationException("Idea not found");
            }

            // Check if user is the owner of the idea
            if (idea.SubmittedByUserId != userGuid)
            {
                throw new InvalidOperationException("You can only delete your own ideas");
            }

            // Only allow deletion of draft ideas
            if (idea.Status != IdeaStatus.Draft && idea.Status != IdeaStatus.UnderReview)
            {
                throw new InvalidOperationException("You can only delete ideas in Draft or UnderReview status");
            }

            dbContext.Ideas.Remove(idea);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
 }
