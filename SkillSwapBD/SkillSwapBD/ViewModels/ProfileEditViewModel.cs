namespace SkillSwapBD.ViewModels
{
    public class ProfileEditViewModel
    {
        public string FullName { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? ExistingImageUrl { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? ExistingCoverImageUrl { get; set; }   
        public IFormFile? CoverImage { get; set; }            
    }
}
