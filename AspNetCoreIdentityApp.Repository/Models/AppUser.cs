using AspNetCoreIdentityApp.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Repository.Models
{
    public class AppUser : IdentityUser
    {
        public string? City { get; set; } // --> ek prop istersem
        public string? Picture { get; set; }
        public DateTime? Birthdate { get; set; }
        public Gender? Gender { get; set; }
    }
}
