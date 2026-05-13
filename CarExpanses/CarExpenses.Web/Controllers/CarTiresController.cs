using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

[Route("[controller]/[action]")]
public class CarTiresController(ICarTireRepository repository, ICarRepository carRepository, ITireRepository tireRepository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    [HttpGet]
    public IActionResult Search(string? query)
    {
        var assignments = repository.GetAll();

        if (string.IsNullOrWhiteSpace(query))
        {
            return PartialView("_CarTireList", assignments);
        }

        var term = query.Trim();
        var filtered = assignments
            .Where(item =>
                item.CarId.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || item.TireId.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || item.InstalledDate.ToString("yyyy-MM-dd").Contains(term, StringComparison.OrdinalIgnoreCase)
                || (item.Car?.Brand?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Car?.Model?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Tire?.Brand?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Tire?.Model?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (item.Tire?.Season?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        return PartialView("_CarTireList", filtered);
    }

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

        repository.Add(new CarTire
        {
            CarId = formModel.CarId,
            TireId = formModel.TireId,
            InstalledDate = formModel.InstalledDate
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var carTire = repository.GetById(id);
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

        var carTire = new CarTire
        {
            Id = formModel.Id,
            CarId = formModel.CarId,
            TireId = formModel.TireId,
            InstalledDate = formModel.InstalledDate
        };

        if (!repository.Update(carTire))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var carTire = repository.GetById(id);
        return carTire is null ? NotFound() : View(carTire);
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


