using System.ComponentModel.DataAnnotations;

namespace SkillSwapBD.ViewModels
{
    public class ReviewFormViewModel
    {
        public int SwapRequestId { get; set; }

        [Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5.")]
        public int Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}