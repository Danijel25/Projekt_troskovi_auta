using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

public class UsersController(IUserRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    [HttpGet]
    public IActionResult Search(string? query)
    {
        var users = repository.GetAll();

        if (string.IsNullOrWhiteSpace(query))
        {
            return PartialView("_UserList", users);
        }

        var term = query.Trim();
        var filtered = users
            .Where(user =>
                user.Username.Contains(term, StringComparison.OrdinalIgnoreCase)
                || user.Email.Contains(term, StringComparison.OrdinalIgnoreCase)
                || user.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return PartialView("_UserList", filtered);
    }

    public IActionResult Details(int id)
    {
        var user = repository.GetById(id);
        return user is null ? NotFound() : View(user);
    }
    [HttpGet]
    public IActionResult Create() => View("Form", new UserFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", formModel);
        }

        repository.Add(new User
        {
            Username = formModel.Username,
            Email = formModel.Email,
            Password = formModel.Password
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var user = repository.GetById(id);
        return user is null ? NotFound() : View("Form", new UserFormViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Password = user.Password
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, UserFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", formModel);
        }

        var user = new User
        {
            Id = formModel.Id,
            Username = formModel.Username,
            Email = formModel.Email,
            Password = formModel.Password
        };

        if (!repository.Update(user))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var user = repository.GetByIdWithDetails(id);
        return user is null ? NotFound() : View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!repository.Delete(id))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

}


