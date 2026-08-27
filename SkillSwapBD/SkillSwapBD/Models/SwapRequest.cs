using static SkillSwapBD.Models.Enums;

namespace SkillSwapBD.Models
{
    public class SwapRequest
    {
        public int Id { get; set; }
        public string RequesterId { get; set; }
        public ApplicationUser Requester { get; set; }
        public string ReceiverId { get; set; }
        public ApplicationUser Receiver { get; set; }
        public int RequesterSkillId { get; set; }
        public Skill RequesterSkill { get; set; }
        public int ReceiverSkillId { get; set; }
        public Skill ReceiverSkill { get; set; }
        public SwapStatus Status { get; set; } = SwapStatus.Pending;
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
