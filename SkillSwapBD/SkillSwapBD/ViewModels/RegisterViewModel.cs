using System.ComponentModel.DataAnnotations;

namespace SkillSwapBD.ViewModels
{
    public class RegisterViewModel
    {
        [Required] public string FullName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
        [DataType(DataType.Password), Compare("Password")] public string ConfirmPassword { get; set; }
    }
}
