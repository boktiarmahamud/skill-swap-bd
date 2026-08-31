using static SkillSwapBD.Models.Enums;

namespace SkillSwapBD.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public SkillType Type { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsApproved { get; set; } = false;   
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SkillLike> Likes { get; set; } = new List<SkillLike>();
        public ICollection<SkillComment> Comments { get; set; } = new List<SkillComment>();
    }
}
