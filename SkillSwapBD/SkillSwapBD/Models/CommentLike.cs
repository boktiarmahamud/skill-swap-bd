namespace SkillSwapBD.Models
{
    public class CommentLike
    {
        public int Id { get; set; }
        public int SkillCommentId { get; set; }
        public SkillComment SkillComment { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
