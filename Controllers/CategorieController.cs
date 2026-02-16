using backend_trial.Data;
using backend_trial.Models.Domain;
using backend_trial.Models.DTO;
using backend_trial.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_trial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorieController : ControllerBase
    {
        private readonly ICategorieRepository categorieRepository;
        private readonly IdeaBoardDbContext dbContext;

        public CategorieController(ICategorieRepository categorieRepository, IdeaBoardDbContext dbContext)
        {
            this.categorieRepository = categorieRepository;
            this.dbContext = dbContext;
        }

        // Get all categories
        [HttpGet("categories")]
        public async Task<ActionResult> GetAllCategories()
        {
            try
            {
                var categories = await categorieRepository.GetAllRepositoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving categories", Error = ex.Message });
            }
        }

        // Get category by ID
        [HttpGet("categories/{id}")]
        public async Task<ActionResult> GetCategoryById(Guid id)
        {
            try
            {
                var category = await categorieRepository.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error retrieving category", Error = ex.Message });
            }
        }

        // Add new category
        [HttpPost("categories")]
        public async Task<ActionResult> AddCategory([FromBody] CategoryRequestDto categoryRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var newCategory = await categorieRepository.AddCategoryAsync(categoryRequestDto);

                return CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.CategoryId }, newCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error adding category", Error = ex.Message });
            }
        }

        // Update category
        [HttpPut("categories/{id}")]
        public async Task<ActionResult> UpdateCategoryAsync(Guid id, [FromBody] CategoryRequestDto categoryRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedCategory = await categorieRepository.UpdateCategoryAsync(id, categoryRequestDto);
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error updating category", Error = ex.Message });
            }

        }

        // Toggle category active/inactive status
        [HttpPatch("categories/{id}/toggle-status")]
        public async Task<ActionResult> ToggleCategoryStatus(Guid id)
        {
            try
            {
                var category = await categorieRepository.ToggleCategoryStatusAsync(id);

                return Ok(new { Message = $"Category is now {(category.IsActive ? "active" : "inactive")}", Category = category });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error toggling category status", Error = ex.Message });
            }
        }

        // Delete category
        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var category = await categorieRepository.DeleteCategoryAsync(id);
                return Ok(new { Message = "Category deleted successfully", Category = category});
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error deleting category", Error = ex.Message });
            }
        }
    }
}

