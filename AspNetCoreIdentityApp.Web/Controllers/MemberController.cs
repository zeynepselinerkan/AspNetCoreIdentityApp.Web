using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;
using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Service.Services;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize] // Sadece üyelerin erişebilmesini istediğim controller ya da methodlara bunu koyabilirim.
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private string userName => User.Identity.Name; // sadece geti olan bir prop => olunca
        private readonly IMemberService _memberService;
        private readonly IHttpContextAccessor _httpContextAccessor;// ctora eklerim dışarıdan claime erişmek için. program cs e ekle : builder.Services.AddHttpContextAccessor();

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
        {

            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }
        public async Task<IActionResult> Index()
        {
            //var userClaims = HttpContext.User.Claims; Controllerda olmayan nesne ile ulaşmak istersem. Örneğin Businessta bir classtan erişmeye çalıştığında. Kritik datalar cookiede tutulmaz. Claimler kullanıcı hakkında tuttuğumuz ve yetkilendirmek için kullandığımız datalardır. Key-Value olarak tutulur.
            // Claimleri okuma şekli :
            //var userClaims = User.Claims.ToList();
            //var email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            return View(await _memberService.GetUserViewModelByUserNameAsync(userName));
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
            await _memberService.LogoutAsync();
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

            if (!await _memberService.CheckPasswordAsync(userName, request.OldPassword))
            {
                ModelState.AddModelError(string.Empty, "Your old password is wrong.");
                return View();
            }

            var (isSuccess, errors) = await _memberService.ChangePasswordAsync(userName, request.NewPassword, request.OldPassword);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);
                return View();
            }



            TempData["SuccessMessage"] = "Your password was changed successfully.";

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.gender = _memberService.GetGenderSelectList();

            return View(await _memberService.GetEditUserViewModelAsync(userName));
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(EditUserViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var (isSuccess, errors) = await _memberService.EditUserAsync(request, userName);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors);
                return View();
            }

            TempData["SuccessMessage"] = "Your information was changed successfully.";

            return View(await _memberService.GetEditUserViewModelAsync(userName));
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            string message = string.Empty;
            message = "You do not have authorization to see this page.";
            ViewBag.message = message;
            return View();
        }
        [HttpGet]
        public IActionResult Claims()
        {
            return View(_memberService.GetClaims(User));
        }

        [Authorize(Policy = "KocaeliPolicy")]
        [HttpGet]
        public IActionResult KocaeliPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }
        [Authorize(Policy = "ViolencePolicy")]
        [HttpGet]
        public IActionResult ViolencePage()
        {
            return View();
        }
    }
}
