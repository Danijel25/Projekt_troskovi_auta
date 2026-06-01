using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class UsersController(UserManager<User> userManager, IUserRepository userRepository) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userRepository.GetListAsync(new UserFilter());
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        var users = await userRepository.GetListAsync(new UserFilter { Search = query });
        return PartialView("_UserList", users);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return user is null ? NotFound() : View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return user is null ? NotFound() : View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound();
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(user);
        }

        return RedirectToAction(nameof(Index));
    }
}


