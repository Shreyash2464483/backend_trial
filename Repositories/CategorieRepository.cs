using backend_trial.Data;
using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
namespace backend_trial.Repositories
{
    public class CategorieRepository : ICategorieRepository
    {
        private readonly IdeaBoardDbContext dbContext;
        public CategorieRepository(IdeaBoardDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Category>> GetAllRepositoriesAsync()
        {
            return await dbContext.Categories.ToListAsync();
        }

        public async Task<Category> AddCategoryAsync([FromBody] CategoryRequestDto categoryRequestDto)
        {
            // Check if category name already exists
            var existingCategory = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryRequestDto.Name.ToLower());

            if (existingCategory != null)
            {
                throw new InvalidOperationException("Category with this name already exists");
            }

            var newCategory = new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = categoryRequestDto.Name,
                Description = categoryRequestDto.Description,
                IsActive = categoryRequestDto.IsActive
            };

            dbContext.Categories.Add(newCategory);
            await dbContext.SaveChangesAsync();

            return newCategory;
        }

        public async Task<Category> UpdateCategoryAsync(Guid id, [FromBody] CategoryRequestDto categoryRequestDto)
        {
            var currentCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (currentCategory == null)
            {
                throw new InvalidOperationException("Category not found!");
            }

            var existingCategory = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryRequestDto.Name.ToLower() && c.CategoryId != id);

            if (existingCategory != null)
            {
                throw new InvalidOperationException("Another category with this name already exists");
            }

            
            currentCategory.Name = categoryRequestDto.Name;
            currentCategory.Description = categoryRequestDto.Description;
            currentCategory.IsActive = categoryRequestDto.IsActive;

            dbContext.Categories.Update(currentCategory);
            await dbContext.SaveChangesAsync();

            return existingCategory;
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                throw new InvalidOperationException("Category not found");
            }
            return category;
        }

        public async Task<Category> DeleteCategoryAsync(Guid id)
        {
            var currentCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (currentCategory == null)
            {
                throw new InvalidOperationException("Category does not exist");
            }

            var ideaUsingCategory = await dbContext.Ideas.AnyAsync(i => i.CategoryId == id);
            if (ideaUsingCategory)
            {
                throw new InvalidOperationException("Cannot delete category as it is being used by ideas.");
            }

            dbContext.Categories.Remove(currentCategory);
            await dbContext.SaveChangesAsync();
            return currentCategory;
        }
    
        public async Task<Category> ToggleCategoryStatusAsync(Guid id)
        {
            var currentCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (currentCategory == null)
            {
                throw new InvalidOperationException("Category not found!");
            }
            currentCategory.IsActive = !currentCategory.IsActive;

            dbContext.Categories.Update(currentCategory);
            await dbContext.SaveChangesAsync();
            return currentCategory;
        }
    }
}
