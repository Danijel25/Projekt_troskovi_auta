using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

[Route("auti")]
public class CarsController(ICarRepository carRepository, IUserRepository userRepository) : Controller
{
    [Route("svi")]
    public IActionResult Index()
    {
        var cars = carRepository.GetAll();
        return View(cars);
    }

    [HttpGet]
    [Route("pretraga")]
    public IActionResult Search(string? query)
    {
        var cars = carRepository.GetAll();

        if (string.IsNullOrWhiteSpace(query))
        {
            return PartialView("_CarList", cars);
        }

        var term = query.Trim();
        var filteredCars = cars
            .Where(car =>
                car.Brand.Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.Model.Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.FuelType.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.Year.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.EngineVolume.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.CurrentMilage.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return PartialView("_CarList", filteredCars);
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

    [HttpGet]
    [Route("novi")]
    public IActionResult Create()
    {
        return View(BuildFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("novi")]
    public IActionResult Create(CarFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View(BuildFormModel(formModel));
        }

        var car = new Car
        {
            UserId = formModel.UserId,
            Brand = formModel.Brand,
            Model = formModel.Model,
            Year = formModel.Year,
            EngineVolume = formModel.EngineVolume,
            CurrentMilage = formModel.CurrentMilage,
            PurchasePrice = formModel.PurchasePrice,
            PurchaseDate = formModel.PurchaseDate,
            FuelType = formModel.FuelType
        };

        carRepository.Add(car);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("uredi/{id}")]
    public IActionResult Edit(int id)
    {
        var car = carRepository.GetById(id);

        if (car is null)
        {
            return NotFound();
        }

        return View(BuildFormModel(car));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("uredi/{id}")]
    public IActionResult Edit(int id, CarFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(BuildFormModel(formModel));
        }

        var car = new Car
        {
            Id = formModel.Id,
            UserId = formModel.UserId,
            Brand = formModel.Brand,
            Model = formModel.Model,
            Year = formModel.Year,
            EngineVolume = formModel.EngineVolume,
            CurrentMilage = formModel.CurrentMilage,
            PurchasePrice = formModel.PurchasePrice,
            PurchaseDate = formModel.PurchaseDate,
            FuelType = formModel.FuelType
        };

        if (!carRepository.Update(car))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("obrisi/{id}")]
    public IActionResult Delete(int id)
    {
        var car = carRepository.GetById(id);

        if (car is null)
        {
            return NotFound();
        }

        return View(car);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("obrisi/{id}")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!carRepository.Delete(id))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private CarFormViewModel BuildFormModel(Car? car = null)
    {
        return new CarFormViewModel
        {
            Id = car?.Id ?? 0,
            UserId = car?.UserId ?? 0,
            Brand = car?.Brand ?? string.Empty,
            Model = car?.Model ?? string.Empty,
            Year = car?.Year ?? DateTime.UtcNow.Year,
            EngineVolume = car?.EngineVolume ?? 0,
            CurrentMilage = car?.CurrentMilage ?? 0,
            PurchasePrice = car?.PurchasePrice ?? 0,
            PurchaseDate = car?.PurchaseDate ?? DateTime.Today,
            FuelType = car?.FuelType ?? FuelType.Petrol,
            UserOptions = userRepository.GetAll()
                .OrderBy(user => user.Username)
                .Select(user => new SelectListItem($"{user.Username} ({user.Email})", user.Id.ToString()))
                .ToList()
        };
    }

    private CarFormViewModel BuildFormModel(CarFormViewModel formModel)
    {
        formModel.UserOptions = userRepository.GetAll()
            .OrderBy(user => user.Username)
            .Select(user => new SelectListItem($"{user.Username} ({user.Email})", user.Id.ToString()))
            .ToList();

        return formModel;
    }

}


