using System.ComponentModel.DataAnnotations;

namespace backend_trial.Models.DTO.Idea
{
    public class IdeaRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 100 characters")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryId { get; set; }
    }
}
