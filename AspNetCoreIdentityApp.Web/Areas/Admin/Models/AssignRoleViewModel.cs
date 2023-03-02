namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class AssignRoleViewModel
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool HasRole  { get; set; }
    }
}
