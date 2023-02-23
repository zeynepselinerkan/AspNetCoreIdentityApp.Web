using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
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
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage ="Email is not in the correct format.")]
        [Required(ErrorMessage = "Email field must be filled.")]
        [Display(Name = "Email :")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Phone number field must be filled.")]
        [Display(Name = "Phone Number :")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password field must be filled.")]
        [Display(Name = "Password :")]
        [MinLength(6, ErrorMessage = "Password must be min 6 characters.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords are not identical.")]
        [Required(ErrorMessage = "Password Confirmation field must be filled.")]
        [Display(Name = "Password Confirmation :")]
        [MinLength(6, ErrorMessage = "Password must be min 6 characters.")]
        public string PasswordConfirmation { get; set; }

    }
}
