using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace backend_trial.Repositories
{
    public interface ICategorieRepository
    {
        Task<List<Category>> GetAllRepositoriesAsync();
        Task<Category> GetCategoryByIdAsync(Guid id);
        Task<Category> AddCategoryAsync(CategoryRequestDto categoryRequestDto);
        Task<Category> UpdateCategoryAsync(Guid id, CategoryRequestDto categoryRequestDto);
        Task<Category> DeleteCategoryAsync(Guid id);
        Task<Category> ToggleCategoryStatusAsync(Guid id);
    }
}
