using CarExpanses.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpanses.Controllers;

public class CarsController(CarMockRepository carRepository) : Controller
{
    public IActionResult Index()
    {
        var cars = carRepository.GetAll();
        return View(cars);
    }

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
