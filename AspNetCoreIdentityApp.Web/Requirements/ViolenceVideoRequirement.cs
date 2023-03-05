using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ViolenceVideoRequirement : IAuthorizationRequirement
    {
        public int ThresholdAge { get; set; }

    }
    public class ViolenceVideoRequirementHandler : AuthorizationHandler<ViolenceVideoRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceVideoRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "Birthdate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            Claim birthdateClaim = context.User.FindFirst("Birthdate")!;

            var today = DateTime.Now;
            var birthdate = Convert.ToDateTime(birthdateClaim.Value);
            var age = today.Year - birthdate.Year;

            if (birthdate > today.AddYears(-age)) age--; // artık yıl hesabı şubat 28-29

            if (requirement.ThresholdAge>age)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
