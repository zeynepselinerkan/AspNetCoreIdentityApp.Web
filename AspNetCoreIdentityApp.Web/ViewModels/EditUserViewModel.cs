using AspNetCoreIdentityApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class EditUserViewModel
    {
        [Required(ErrorMessage = "Username field must be filled.")]
        [Display(Name = "Username :")]
        public string UserName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email is not in the correct format.")]
        [Required(ErrorMessage = "Email field must be filled.")]
        [Display(Name = "Email :")]
        public string EmailAddress { get; set; } = null!;

        [Required(ErrorMessage = "Phone number field must be filled.")]
        [Display(Name = "Phone Number :")]
        public string PhoneNumber { get; set; } = null!;

        [Display(Name = "City :")]
        public string? City { get; set; }

        [Display(Name = "Profile Picture :")]
        public IFormFile? Picture { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birthdate :")]
        public DateTime? Birthdate { get; set; }

        [Display(Name = "Gender :")]
        public Gender? Gender { get; set; }


    }
}
