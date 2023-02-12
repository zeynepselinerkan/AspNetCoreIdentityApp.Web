using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;



namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();
            var isNumeric = int.TryParse(user.UserName![0].ToString(),out _);

            if (isNumeric)
            {
                errors.Add(new() { Code = "UserNameContainsFirstLetterNumeric", Description = "First character of user name can not contain numeric character." });
            }
            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
