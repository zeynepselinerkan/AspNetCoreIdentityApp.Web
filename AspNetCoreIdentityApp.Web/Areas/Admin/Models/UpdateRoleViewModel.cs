using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class UpdateRoleViewModel
    {
        public  string Id { get; set; }

        [Required(ErrorMessage = "Role name can not be empty.")]
        [Display(Name = "Role Name :")]
        public string Name { get; set; }
    }
}
