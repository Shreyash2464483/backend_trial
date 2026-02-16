using backend_trial.Models.Domain;
using backend_trial.Models.DTO;

namespace backend_trial.Repositories
{
    public interface ICommentRepository
    {
        Task<CommentResponseDto> AddCommentAsync(Guid ideaId, Guid userGuid, CommentRequestDto request);
        Task<List<CommentResponseDto>> GetCommentsForIdeaAsync(Guid ideaId);
        Task<CommentResponseDto> GetCommentByIdAsync(Guid id);
        Task<CommentResponseDto> UpdateCommentAsync(Guid id, Guid userGuid, CommentRequestDto request);
        Task<bool> DeleteCommentAsync(Guid id, Guid userGuid);
    }
}
