using CarExpenses.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

public class UsersController(IUserRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var user = repository.GetById(id);
        return user is null ? NotFound() : View(user);
    }
}


