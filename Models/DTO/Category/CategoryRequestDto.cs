using System.ComponentModel.DataAnnotations;

namespace backend_trial.Models.DTO.Category
{
    public class CategoryRequestDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
    }
}
