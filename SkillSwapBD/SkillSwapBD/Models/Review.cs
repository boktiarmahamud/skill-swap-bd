namespace SkillSwapBD.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int SwapRequestId { get; set; }
        public SwapRequest SwapRequest { get; set; }
        public string ReviewerId { get; set; }
        public ApplicationUser Reviewer { get; set; }
        public string RevieweeId { get; set; }
        public ApplicationUser Reviewee { get; set; }
        public int Rating { get; set; } // 1–5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
