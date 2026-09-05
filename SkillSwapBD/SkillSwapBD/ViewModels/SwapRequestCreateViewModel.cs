using System.ComponentModel.DataAnnotations;
using SkillSwapBD.Models;

namespace SkillSwapBD.ViewModels
{
    public class SwapRequestCreateViewModel
    {
        public int ReceiverSkillId { get; set; }
        public Skill? ReceiverSkill { get; set; }

        [Required(ErrorMessage = "Pick one of your own skills to offer in return.")]
        [Display(Name = "One of my skills to offer")]
        public int RequesterSkillId { get; set; }

        public List<Skill> MyOfferSkills { get; set; } = new();

        [StringLength(500)]
        [Display(Name = "Message (optional)")]
        public string? Message { get; set; }
    }
}