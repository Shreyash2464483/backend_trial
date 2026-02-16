using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace backend_trial.Repositories
{
    public interface IIdeaRepository
    {
        Task<List<IdeaResponseDto>> GetAllIdeasAsync();
        Task<List<IdeaResponseDto>> GetMyIdeasAsync(Guid userGuid);
        Task<IdeaResponseDto> GetIdeaByIdAsync(Guid id);
        Task<IdeaResponseDto> SubmitIdeaAsync(IdeaRequestDto ideaRequestDto, Guid userGuid);
        Task<IdeaResponseDto> UpdateIdeaAsync(Guid id, IdeaRequestDto ideaRequestDto, Guid userGuid);
        Task<bool> DeleteIdeaAsync(Guid id, Guid userGuid);
    }
}
