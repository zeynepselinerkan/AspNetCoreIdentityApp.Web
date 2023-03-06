using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }
        public async Task<UserUserViewModel> GetUserViewModelByUserNameAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            return new UserUserViewModel
            {
                Email = currentUser!.Email,
                Username = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        // Tuple<> --> <()>

        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;

            return await _userManager.CheckPasswordAsync(currentUser, password);
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string newPassword, string oldPassword)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;
            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            if (!resultChangePassword.Succeeded)
            {
                return (false, resultChangePassword.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, newPassword, true, false);

            return (true, null);
        }
        // Burada password gelmez password hash olarak gelir. currentUser...
        public async Task<EditUserViewModel> GetEditUserViewModelAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            return new EditUserViewModel()
            {
                UserName = currentUser.UserName,
                EmailAddress = currentUser.Email,
                PhoneNumber = currentUser.PhoneNumber,
                Birthdate = currentUser.Birthdate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };
        }
        SelectList IMemberService.GetGenderSelectList()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> EditUserAsync(EditUserViewModel request, string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

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

                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;

            }
            var updateToUserResult = await _userManager.UpdateAsync(currentUser);
            if (!updateToUserResult.Succeeded)
            {
                return (false, updateToUserResult.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync(); // Cookie yeniden oluşsun diye logout yaptırıyoruz.

            if (request.Birthdate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("Birthdate", currentUser.Birthdate.Value.ToString()) });
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);
            }

            return (true, null);
        }

        public List<ClaimViewModel> GetClaims(ClaimsPrincipal principal)
        {
            //User.Identity.Name == User.Claims.First(x => x.Type == ClaimTypes.Name);

            return principal.Claims.Select(x => new ClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();
        }
    }
}
