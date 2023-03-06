using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Core.PermissionRoot;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Security;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Seeds
{
    public class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
            var hasAdvanceRole = await roleManager.RoleExistsAsync("AdvanceRole");
            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });

                var basicRole =await roleManager.FindByNameAsync("BasicRole");

                await AddReadPermission(basicRole,roleManager);

            }
            if (!hasAdvanceRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdvanceRole" });

                var advanceRole = await roleManager.FindByNameAsync("AdvanceRole");

                await AddReadPermission(advanceRole, roleManager);
                await AddUpdateAndCreatePermission(advanceRole, roleManager);


            }
            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdminRole" });

                var adminRole = await roleManager.FindByNameAsync("AdminRole");

                await AddReadPermission(adminRole, roleManager);
                await AddUpdateAndCreatePermission(adminRole, roleManager);
                await AddDeletePermission(adminRole, roleManager);

            }
        }

        public static async Task AddReadPermission(AppRole role,RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Stock.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Order.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Catalog.Read));
        }
        public static async Task AddUpdateAndCreatePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Stock.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Order.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Catalog.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Stock.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Order.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Catalog.Update));
        }
        public static async Task AddDeletePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Stock.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Order.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permission.Catalog.Delete));
        }

    }
}
