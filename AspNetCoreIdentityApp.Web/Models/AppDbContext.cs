using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Models
{
    public class AppDbContext : IdentityDbContext<AppUser,AppRole,string> // string diyince id için random guid değerler üretecek.
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {

        }
    }
}
