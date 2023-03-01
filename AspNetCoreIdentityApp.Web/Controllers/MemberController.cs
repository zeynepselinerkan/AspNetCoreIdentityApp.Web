using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize] // Sadece üyelerin erişebilmesini istediğim controller ya da methodlara bunu koyabilirim.
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {

            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userViewModel = new UserUserViewModel
            {
                Email = currentUser!.Email,
                Username = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
            };

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
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.OldPassword);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Your old password is wrong.");
                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.OldPassword, request.NewPassword);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x => x.Description).ToList());
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, request.NewPassword, true, false);

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
                Gender = currentUser.Gender
            };

            return View(userEditViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(EditUserViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            currentUser.UserName = request.UserName;
            currentUser.Email = request.EmailAddress;
            currentUser.Birthdate = request.Birthdate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;
            currentUser.PhoneNumber = request.PhoneNumber;

            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}"; // .jpg gibi alacak.

                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath,randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;

            }
            var updateToUserResult = await _userManager.UpdateAsync(currentUser);
            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }
            // Burada password gelmez password hash olarak gelir. currentUser...

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, true);

            TempData["SuccessMessage"] = "Your information was changed successfully.";

            var userEditViewModel = new EditUserViewModel()
            {
                UserName = currentUser.UserName,
                EmailAddress = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                Birthdate = currentUser.Birthdate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };

            return View(userEditViewModel);
        }
    }
}
