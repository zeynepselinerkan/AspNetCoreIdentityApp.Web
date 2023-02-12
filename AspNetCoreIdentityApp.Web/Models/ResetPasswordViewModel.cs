using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Models
{
    public class ResetPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "Email is not in the correct format.")]
        [Required(ErrorMessage = "Email field must be filled.")]
        [Display(Name = "Email :")]
        public string EmailAddress { get; set; }
    }
}
