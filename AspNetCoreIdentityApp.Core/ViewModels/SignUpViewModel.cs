using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel() { }
        public SignUpViewModel(string userName, string emailAddress, string phoneNumber, string password)
        {// ya da yanına soru işareti ekle --> nullable yeşil uyarıları için
            UserName = userName;
            EmailAddress = emailAddress;
            PhoneNumber = phoneNumber;
            Password = password;
        }
        [Required(ErrorMessage ="Username field must be filled.")]
        [Display(Name = "Username :")]
        public string UserName { get; set; } = null!;

        [EmailAddress(ErrorMessage ="Email is not in the correct format.")]
        [Required(ErrorMessage = "Email field must be filled.")]
        [Display(Name = "Email :")]
        public string EmailAddress { get; set; } = null!;

        [Required(ErrorMessage = "Phone number field must be filled.")]
        [Display(Name = "Phone Number :")]
        public string PhoneNumber { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password field must be filled.")]
        [Display(Name = "Password :")]
        [MinLength(6, ErrorMessage = "Password must be min 6 characters.")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords are not identical.")]
        [Required(ErrorMessage = "Password Confirmation field must be filled.")]
        [Display(Name = "Password Confirmation :")]
        [MinLength(6, ErrorMessage = "Password must be min 6 characters.")]
        public string PasswordConfirmation { get; set; } = null!;

    }
}
