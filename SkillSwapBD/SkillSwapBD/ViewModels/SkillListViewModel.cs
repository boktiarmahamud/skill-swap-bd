using SkillSwapBD.Models;
using static SkillSwapBD.Models.Enums;

namespace SkillSwapBD.ViewModels
{
    public class SkillListViewModel
    {
        public List<Skill> Skills { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public SkillType? Type { get; set; }
    }
}