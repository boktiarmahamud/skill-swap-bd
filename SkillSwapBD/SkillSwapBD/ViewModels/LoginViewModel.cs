using System.ComponentModel.DataAnnotations;

namespace SkillSwapBD.ViewModels
{
    public class LoginViewModel
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
