﻿using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Services;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager; //Kullanıcı işlemleri yapmak için gereken sınıf! Readonly deme sebebim sadece constructorda initialize olmasını istiyorum.
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null) // Kullanıcı, sadece üyelerin girebileceği bir sayfaya giderse signIn'den sonra oraya yönlendiricem. Girmek istediği sayfa returnUrl'de tutucam. returnUrl yoksa anasayfaya yönlendiricem. Erişmek istediği sayfayı kaybetmemiş olacağım.
        {
            //HttpContext.Request // uygulamanın kalbi, tüm request ve responselara ulaşırım.

            returnUrl = returnUrl ?? Url.Action("Index", "Home");
            var hasUser = await _userManager.FindByEmailAsync(request.EmailAddress);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong.");
                return View();
            }
            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, true); //true der isem identity default kitleme metodunu işleme alır.Burası başarılı olursa bize cookie oluşturacak.

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "You can not sign in by 3 minutes." });
                return View();
            }

            ModelState.AddModelErrorList(new List<string>() { $"Email or password is wrong.", $" Number Of Failed Access : {await _userManager.GetAccessFailedCountAsync(hasUser)}" });

            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            //Katmanlı mimaride buraya yazılan kodlar Service'de olacak!
            //Servera yük bindiğinde async hızını gösterir.
            //Hash => password hashlendi --> asdadhadkjfhdjkfhkds , bu data geridönüşümsüzdür.Encrypt edilirse Decyrpt edilebilir (anahtarı tanımlanır). Hashlenen veri geriye alamayız.
            //MD5, SHA --> HASHLEME ALGORİTMALARI
            if (!ModelState.IsValid)
            {
                return View();

            }

            var identityResult = await _userManager.CreateAsync(new() { UserName = request.UserName, PhoneNumber = request.PhoneNumber, Email = request.EmailAddress }, request.PasswordConfirmation); // await yazmazsam task sınıfıyla wraplanır(identityResult)

            //if(identityResult.Succeeded)
            //{
            //    ViewBag.SuccessMessage = "Membership registration completed successfully.";
            //    return View();
            //}
            if (identityResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Membership registration completed successfully.";
                return RedirectToAction(nameof(HomeController.SignUp));
            }
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel request)
        {
            var hasUser = await _userManager.FindByEmailAsync(request.EmailAddress);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "This email address can not be found.");
                return View(); // Hatayı redirecttoaction ile taşımak istersem mutlaka temp data ile kısa süreli bir cookie vasıtasıyla taşımalıyım. Bu Http protokolünün bir kuralıdır.
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            // link --> https://localhost:7206?userId=12213&token=Dsadsaf token güvenlik için, ömür de vericem (sadece 2 saat örneğin).

            // Email Service --> Email göndericem.
            await _emailService.SendResetPasswordEmail(passwordResetLink, hasUser.Email);

            TempData["SuccessMessage"] = "Reset password link was sent to your email address";

            return RedirectToAction(nameof(ForgotPassword)); // Viewbag yazarsam buradan tekrar anasayfaya giderse, sayfayı her yenilediğinde tekrar gönderir çünkü bu post method. O yüzden temp data ile beraber diğer requeste data taşımalıyım.
        }
        public IActionResult ResetPassword(string userId,string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            string userId = TempData["userId"].ToString();
            string token = TempData["token"].ToString();

            if (userId==null || token==null)
            {
                throw new Exception("An error occured.");
            }

            var hasUser = await _userManager.FindByIdAsync(userId);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "User can not be found.");
                return View();
            }
            var result = await _userManager.ResetPasswordAsync(hasUser, token, request.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your password was resetted successfully.";
            }

            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
            }

            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}