using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize] // Sadece üyelerin erişebilmesini istediğim controller ya da methodlara bunu koyabilirim.
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {

            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser =await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userViewModel = new UserUserViewModel { 
                Email = currentUser!.Email,
                Username = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber };

            return View(userViewModel);
        }
        // 1.YÖNTEM --> Birden fazla log out butonu olursa farklı yönlendirmek istersem böyle zor. O yüzden 2.yöntem daha iyi!
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Index","Home");
        //}

        //2.YÖNTEM
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
           
        }
    }
}
