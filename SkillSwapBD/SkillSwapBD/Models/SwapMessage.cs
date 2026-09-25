namespace SkillSwapBD.Models
{
    public class SwapMessage
    {
        public int Id { get; set; }
        public int SwapRequestId { get; set; }
        public SwapRequest SwapRequest { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}