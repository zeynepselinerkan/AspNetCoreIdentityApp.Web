using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Old Password field must be filled.")]
        [Display(Name = "Old Password :")]
        [MinLength(6, ErrorMessage = "Password must be min 6 characters.")]
        public string OldPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password field must be filled.")]
        [Display(Name = "New Password :")]
        [MinLength(6, ErrorMessage = "Password must be min 6 characters.")]
        public string NewPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords are not identical.")]
        [Required(ErrorMessage = "Password Confirmation field must be filled.")]
        [Display(Name = "New Password Confirmation :")]
        [MinLength(6, ErrorMessage = "Password must be min 6 characters.")]
        public string NewPasswordConfirm { get; set; } = null!;

    }
}
