using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize] // Sadece üyelerin erişebilmesini istediğim controller ya da methodlara bunu koyabilirim.
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        public MemberController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
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
