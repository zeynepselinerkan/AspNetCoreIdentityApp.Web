using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel(){} // Default ctor, framework requestin bodysindeki datayı bu vm'e mapler.
        public SignInViewModel(string emailAddress, string password)
        {
            EmailAddress = emailAddress;
            Password = password;
        }

        [EmailAddress(ErrorMessage = "Email is not in the correct format.")]
        [Required(ErrorMessage = "Email field must be filled.")]
        [Display(Name = "Email :")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password field must be filled.")]
        [Display(Name = "Password :")]
        public string Password { get; set; }

        [Display(Name ="Remember Me ?")]
        public bool RememberMe { get; set; }
    }
}
