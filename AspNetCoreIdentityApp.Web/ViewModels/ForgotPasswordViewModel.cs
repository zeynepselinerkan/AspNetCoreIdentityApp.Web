using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "Email is not in the correct format.")]
        [Required(ErrorMessage = "Email field must be filled.")]
        [Display(Name = "Email :")]
        public string EmailAddress { get; set; }
    }
}
