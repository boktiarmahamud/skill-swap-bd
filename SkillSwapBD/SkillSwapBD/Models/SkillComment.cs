namespace SkillSwapBD.Models
{
    public class SkillComment
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Content { get; set; }
        public int? ParentCommentId { get; set; }
        public SkillComment ParentComment { get; set; }
        public ICollection<SkillComment> Replies { get; set; } = new List<SkillComment>();
        public ICollection<CommentLike> Likes { get; set; } = new List<CommentLike>();  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
