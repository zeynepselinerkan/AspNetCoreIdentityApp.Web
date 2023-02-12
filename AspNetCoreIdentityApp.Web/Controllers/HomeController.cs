using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Extensions;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager; //Kullanıcı işlemleri yapmak için gereken sınıf! Readonly deme sebebim sadece constructorda initialize olmasını istiyorum.
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
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

            ModelState.AddModelErrorList(new List<string>() { $"Email or password is wrong.",$" Number Of Failed Access : {await _userManager.GetAccessFailedCountAsync(hasUser)}" });
      
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
            ModelState.AddModelErrorList(identityResult.Errors.Select(x=>x.Description).ToList());
     
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}