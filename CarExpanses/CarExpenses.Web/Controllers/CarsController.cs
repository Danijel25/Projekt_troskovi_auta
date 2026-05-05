using CarExpenses.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Route("auti")]
public class CarsController(ICarRepository carRepository) : Controller
{
    [Route("svi")]
    public IActionResult Index()
    {
        var cars = carRepository.GetAll();
        return View(cars);
    }

    [Route("detalji/{id}")]
    public IActionResult Details(int id)
    {
        var car = carRepository.GetById(id);

        if (car is null)
        {
            return NotFound();
        }

        return View(car);
    }
}


