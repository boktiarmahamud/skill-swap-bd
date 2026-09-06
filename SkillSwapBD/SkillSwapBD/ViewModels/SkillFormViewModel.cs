using System.ComponentModel.DataAnnotations;
using SkillSwapBD.Models;
using static SkillSwapBD.Models.Enums;

namespace SkillSwapBD.ViewModels
{
    public class SkillFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        [Required, StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public SkillType Type { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IFormFile? Attachment { get; set; }         // ADD - optional
        public string? ExistingAttachmentUrl { get; set; }  // ADD

        public List<Category> Categories { get; set; } = new();
    }
}