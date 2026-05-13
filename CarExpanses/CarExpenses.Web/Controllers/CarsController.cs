using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

[Route("auti")]
public class CarsController(ICarRepository carRepository, IUserRepository userRepository, CarExpesesDbContext dbContext) : Controller
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

        dbContext.Cars.Add(car);
        dbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("uredi/{id}")]
    public IActionResult Edit(int id)
    {
        var car = dbContext.Cars.AsNoTracking().FirstOrDefault(car => car.Id == id);

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

        var car = dbContext.Cars.FirstOrDefault(car => car.Id == id);

        if (car is null)
        {
            return NotFound();
        }

        car.UserId = formModel.UserId;
        car.Brand = formModel.Brand;
        car.Model = formModel.Model;
        car.Year = formModel.Year;
        car.EngineVolume = formModel.EngineVolume;
        car.CurrentMilage = formModel.CurrentMilage;
        car.PurchasePrice = formModel.PurchasePrice;
        car.PurchaseDate = formModel.PurchaseDate;
        car.FuelType = formModel.FuelType;

        dbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("obrisi/{id}")]
    public IActionResult Delete(int id)
    {
        var car = LoadCarForDelete(id);

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
        var car = LoadCarForDelete(id);

        if (car is null)
        {
            return NotFound();
        }

        dbContext.Cars.Remove(car);
        dbContext.SaveChanges();

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

    private Car? LoadCarForDelete(int id)
    {
        return dbContext.Cars
            .Include(car => car.FuelExpenses)
            .Include(car => car.ServiceRecords)
            .Include(car => car.Insurances)
            .Include(car => car.CarTires)!
                .ThenInclude(carTire => carTire.Tire)
            .Include(car => car.Expenses)!
                .ThenInclude(expense => expense.Category)
            .FirstOrDefault(car => car.Id == id);
    }
}


