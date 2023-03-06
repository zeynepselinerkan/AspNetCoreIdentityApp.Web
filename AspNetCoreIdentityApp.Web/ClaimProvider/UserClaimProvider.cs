using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Transactions;

namespace AspNetCoreIdentityApp.Web.ClaimProvider
{
    public class UserClaimProvider : IClaimsTransformation // Cookilerdeki dataları claim nesnesine dönüştürmeye yarar.
    {
        // Kullanıcı hakkında data olmak zorunda değil. Örneğin apinin istekleri ile ilgili olabilir, abc sitesini yazabilirim.

        private readonly UserManager<AppUser> _userManager;

        public UserClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal) // Her seferinde çalışır her yenilemede.
        {
            var identityUser = principal.Identity as ClaimsIdentity; // Cookiler claimden oluşur. Kullanıcının kimlik bilgisi claimlerden oluşuyor. Claimlerden oluşan bir kimlik.

            var currentUser = await _userManager.FindByNameAsync(identityUser!.Name!);

            // Sadece authorize yetkisine sahip yerlerde çalışır. O yüzden bunu silebilirim.
            //if (currentUser == null)
            //{
            //    return principal;
            //}

            if (String.IsNullOrEmpty(currentUser.City))
            {
                return principal;
            }

            if (principal.HasClaim(x => x.Type != "City"))
            {
                Claim cityClaim = new Claim("City", currentUser.City); // User claim tabloma kaydetmedim çünkü bu data user tablomda var.

                identityUser.AddClaim(cityClaim);
            }

            return principal;

        }
    }
}
