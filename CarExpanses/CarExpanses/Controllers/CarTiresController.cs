using CarExpanses.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpanses.Controllers;

public class CarTiresController(CarTireMockRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var carTire = repository.GetById(id);
        return carTire is null ? NotFound() : View(carTire);
    }
}
