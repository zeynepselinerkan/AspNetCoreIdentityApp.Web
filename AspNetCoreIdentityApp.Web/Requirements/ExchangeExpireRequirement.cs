using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {
        //public  int Age { get; set; } propgram cs e age i taşıyabilirim.
    }
    public class ExchangeExpirationRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement> // Program.cs den buraya parametre göndermek istersem buraya ihtiyacım var. Örneğin 18 yaşından küçükler bu videoyu izleyemez gibi bir durumda gerekecek.
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "ExchangeExpireDate"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            Claim exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate")!;

            if (DateTime.Now > Convert.ToDateTime(exchangeExpireDate.Value))
            {
                context.Fail();
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;

        }
    }

}
