using Microsoft.AspNetCore.Identity;

namespace SkillSwapBD.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRejected { get; set; } = false;
        public bool ApprovalMessageShown { get; set; } = false;
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
