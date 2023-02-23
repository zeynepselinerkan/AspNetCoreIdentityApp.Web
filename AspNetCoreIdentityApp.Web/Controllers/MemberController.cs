using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel request)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var currentUser =(await _userManager.FindByNameAsync(User.Identity!.Name!))!;
            
            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser,request.OldPassword);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Your old password is wrong.");
                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser,request.OldPassword,request.NewPassword);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x=>x.Description).ToList());
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser,request.NewPassword,true,false);

            TempData["SuccessMessage"] = "Your password was changed successfully.";

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {

            ViewBag.gender = new SelectList(Enum.GetNames(typeof(Gender)));
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var userEditViewModel = new EditUserViewModel()
            {
                UserName = currentUser.UserName,
                EmailAddress = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                Birthdate = currentUser.Birthdate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };

            return View(userEditViewModel);
        }
    }
}
