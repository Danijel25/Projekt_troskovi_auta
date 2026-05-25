using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class UsersController(UserManager<User> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users
            .Include(user => user.Cars)
            .AsNoTracking()
            .OrderBy(user => user.Id)
            .ToListAsync();

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        var users = userManager.Users
            .Include(user => user.Cars)
            .AsNoTracking()
            .AsQueryable();

        if (string.IsNullOrWhiteSpace(query))
        {
            return PartialView("_UserList", await users.ToListAsync());
        }

        var term = query.Trim();
        var filtered = await users
            .Where(user =>
                (user.UserName != null && user.UserName.Contains(term))
                || (user.Email != null && user.Email.Contains(term))
                || user.Id.ToString().Contains(term))
            .ToListAsync();

        return PartialView("_UserList", filtered);
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await userManager.Users
            .Include(item => item.Cars)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        return user is null ? NotFound() : View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await userManager.Users
            .Include(item => item.Cars)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

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


