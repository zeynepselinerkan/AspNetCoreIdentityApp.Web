using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class ResetPasswordViewModel // API'deki DTO
    {
        [Required(ErrorMessage = "Password field must be filled.")]
        [Display(Name = "New Password :")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords are not identical.")]
        [Required(ErrorMessage = "Password Confirmation field must be filled.")]
        [Display(Name = "New Password Confirmation :")]
        public string PasswordConfirmation { get; set; }
    }
}
