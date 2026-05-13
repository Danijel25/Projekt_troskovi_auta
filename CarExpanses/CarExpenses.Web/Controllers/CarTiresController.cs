using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

[Route("[controller]/[action]")]
public class CarTiresController(ICarTireRepository repository, ICarRepository carRepository, ITireRepository tireRepository, CarExpesesDbContext dbContext) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var carTire = repository.GetById(id);
        return carTire is null ? NotFound() : View(carTire);
    }

    [HttpGet]
    public IActionResult Create() => View("Form", BuildFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CarTireFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        dbContext.CarTires.Add(new CarTire
        {
            CarId = formModel.CarId,
            TireId = formModel.TireId,
            InstalledDate = formModel.InstalledDate
        });

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var carTire = dbContext.CarTires.AsNoTracking().FirstOrDefault(item => item.Id == id);
        return carTire is null ? NotFound() : View("Form", BuildFormModel(carTire));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, CarTireFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        var carTire = dbContext.CarTires.FirstOrDefault(item => item.Id == id);
        if (carTire is null)
        {
            return NotFound();
        }

        carTire.CarId = formModel.CarId;
        carTire.TireId = formModel.TireId;
        carTire.InstalledDate = formModel.InstalledDate;

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var carTire = dbContext.CarTires
            .Include(item => item.Car)
            .Include(item => item.Tire)
            .FirstOrDefault(item => item.Id == id);

        return carTire is null ? NotFound() : View(carTire);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var carTire = dbContext.CarTires
            .Include(item => item.Car)
            .Include(item => item.Tire)
            .FirstOrDefault(item => item.Id == id);

        if (carTire is null)
        {
            return NotFound();
        }

        dbContext.CarTires.Remove(carTire);
        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    private CarTireFormViewModel BuildFormModel(CarTire? carTire = null)
    {
        return new CarTireFormViewModel
        {
            Id = carTire?.Id ?? 0,
            CarId = carTire?.CarId ?? 0,
            TireId = carTire?.TireId ?? 0,
            InstalledDate = carTire?.InstalledDate ?? DateTime.Today,
            CarOptions = carRepository.GetAll()
                .OrderBy(car => car.Brand)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
                .ToList(),
            TireOptions = tireRepository.GetAll()
                .OrderBy(tire => tire.Brand)
                .ThenBy(tire => tire.Model)
                .Select(tire => new SelectListItem($"{tire.Brand} {tire.Model} ({tire.Season})", tire.Id.ToString()))
                .ToList()
        };
    }

    private CarTireFormViewModel BuildFormModel(CarTireFormViewModel formModel)
    {
        formModel.CarOptions = carRepository.GetAll()
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
            .ToList();
        formModel.TireOptions = tireRepository.GetAll()
            .OrderBy(tire => tire.Brand)
            .ThenBy(tire => tire.Model)
            .Select(tire => new SelectListItem($"{tire.Brand} {tire.Model} ({tire.Season})", tire.Id.ToString()))
            .ToList();
        return formModel;
    }
}


