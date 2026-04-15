using CarExpanses.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpanses.Controllers;

public class UsersController(UserMockRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var user = repository.GetById(id);
        return user is null ? NotFound() : View(user);
    }
}
