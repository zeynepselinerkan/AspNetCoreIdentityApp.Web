using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class ModelStateExtension
    {
        public static void AddModelErrorList(this ModelStateDictionary modelState,List<string> errors)
        {

            errors.ForEach(x =>
            {
                modelState.AddModelError(string.Empty,x);
            });
               
            
        }
    }
}
