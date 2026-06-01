using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class CarTiresController(ICarTireRepository repository, ICarRepository carRepository, ITireRepository tireRepository) : Controller
{
    public async Task<IActionResult> Index() => View(await repository.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        var assignments = await repository.GetListAsync(new CarTireFilter { Search = query });
        return PartialView("_CarTireList", assignments);
    }

    public async Task<IActionResult> Details(int id)
    {
        var carTire = await repository.GetByIdAsync(id);
        return carTire is null ? NotFound() : View(carTire);
    }

    [HttpGet]
    public async Task<IActionResult> Create() => View("Form", await BuildFormModelAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CarTireFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModelAsync(formModel));
        }

        if (await carRepository.GetByIdAsync(formModel.CarId) is null)
        {
            ModelState.AddModelError(nameof(formModel.CarId), "Car not found.");
            return View("Form", BuildFormModelAsync(formModel));
        }

        if (tireRepository.GetByIdAsync(formModel.TireId) is null)
        {
            ModelState.AddModelError(nameof(formModel.TireId), "Tire not found.");
            return View("Form", BuildFormModelAsync(formModel));
        }

        await repository.AddAsync(new CarTire
        {
            CarId = formModel.CarId,
            TireId = formModel.TireId,
            InstalledDate = formModel.InstalledDate
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var carTire = await repository.GetByIdAsync(id);
        return carTire is null ? NotFound() : View("Form", await BuildFormModelAsync(carTire));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CarTireFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModelAsync(formModel));
        }

        if (await carRepository.GetByIdAsync(formModel.CarId) is null)
        {
            ModelState.AddModelError(nameof(formModel.CarId), "Car not found.");
            return View("Form", BuildFormModelAsync(formModel));
        }

        if (tireRepository.GetByIdAsync(formModel.TireId) is null)
        {
            ModelState.AddModelError(nameof(formModel.TireId), "Tire not found.");
            return View("Form", BuildFormModelAsync(formModel));
        }

        var carTire = new CarTire
        {
            Id = formModel.Id,
            CarId = formModel.CarId,
            TireId = formModel.TireId,
            InstalledDate = formModel.InstalledDate
        };

        if (!await repository.UpdateAsync(carTire))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var carTire = await repository.GetByIdAsync(id);
        return carTire is null ? NotFound() : View(carTire);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await repository.DeleteAsync(id))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<CarTireFormViewModel> BuildFormModelAsync(CarTire? carTire = null)
    {
        return new CarTireFormViewModel
        {
            Id = carTire?.Id ?? 0,
            CarId = carTire?.CarId ?? 0,
            TireId = carTire?.TireId ?? 0,
            InstalledDate = carTire?.InstalledDate ?? DateTime.Today,
            CarOptions = (await carRepository.GetAllAsync())
                .OrderBy(car => car.Brand)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
                .ToList(),
            TireOptions = (await tireRepository.GetAllAsync())
                .OrderBy(tire => tire.Brand)
                .ThenBy(tire => tire.Model)
                .Select(tire => new SelectListItem($"{tire.Brand} {tire.Model} ({tire.Season})", tire.Id.ToString()))
                .ToList()
        };
    }

    private async Task<CarTireFormViewModel> BuildFormModelAsync(CarTireFormViewModel formModel)
    {
        formModel.CarOptions = (await carRepository.GetAllAsync())
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
            .ToList();
        formModel.TireOptions = (await tireRepository.GetAllAsync())
            .OrderBy(tire => tire.Brand)
            .ThenBy(tire => tire.Model)
            .Select(tire => new SelectListItem($"{tire.Brand} {tire.Model} ({tire.Season})", tire.Id.ToString()))
            .ToList();
        return formModel;
    }
}


