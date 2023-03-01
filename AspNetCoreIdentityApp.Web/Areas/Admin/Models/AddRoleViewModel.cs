using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class AddRoleViewModel
    {
        [Required(ErrorMessage = "Role name can not be empty.")]
        [Display(Name = "Role Name :")]
        public string Name { get; set; }
    }
}
