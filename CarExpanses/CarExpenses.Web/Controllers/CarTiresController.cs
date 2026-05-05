using CarExpenses.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Route("[controller]/[action]")]
public class CarTiresController(ICarTireRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var carTire = repository.GetById(id);
        return carTire is null ? NotFound() : View(carTire);
    }
}


