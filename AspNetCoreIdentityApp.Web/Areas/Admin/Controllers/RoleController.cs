using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager = null)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
          var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();

            return View(roles);
        }
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name=request.Name });
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }
            TempData["SuccessMessage"] = "Role was created successfully.";
            return RedirectToAction(nameof(RoleController.Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRole(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if (roleToUpdate==null)
            {
                throw new Exception("Id to update was not found."); // Exceptionlar Service katmanında tanımlanır !!
            }

            return View(new UpdateRoleViewModel() { Id=roleToUpdate.Id, Name=roleToUpdate!.Name!});
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRole(UpdateRoleViewModel request)
        {

            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            roleToUpdate.Name = request.Name;

            if (roleToUpdate==null)
            {
                throw new Exception("Id to update was not found."); // Exceptionlar Service katmanında tanımlanır !!
            }

            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Role was updated successfully.";

            return View();
        }
        public async Task<IActionResult> DeleteRole(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);
           

            if (roleToDelete == null)
            {
                throw new Exception("Id to delete was not found."); // Exceptionlar Service katmanında tanımlanır !!
            }

            var result =await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }

            TempData["SuccessMessage"] = "Role was deleted successfully.";
            return RedirectToAction(nameof(RoleController.Index));
        }
    }
}
