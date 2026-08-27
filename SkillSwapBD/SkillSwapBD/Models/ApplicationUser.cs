using Microsoft.AspNetCore.Identity;

namespace SkillSwapBD.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
