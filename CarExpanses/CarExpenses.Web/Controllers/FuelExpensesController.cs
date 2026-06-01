using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

[Authorize]
[Route("troskovi-goriva/[action]")]
public class FuelExpensesController(IFuelExpenseRepository repository, ICarRepository carRepository) : Controller
{
    public async Task<IActionResult> Index() => View(await repository.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        var fuelExpenses = await repository.GetListAsync(new FuelExpenseFilter { Search = query });
        return PartialView("_FuelExpenseList", fuelExpenses);
    }

    public async Task<IActionResult> Details(int id)
    {
        var fuelExpense = await repository.GetByIdAsync(id);
        return fuelExpense is null ? NotFound() : View(fuelExpense);
    }

    [HttpGet]
    public async Task<IActionResult> Create() => View("Form", await BuildFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FuelExpenseFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        if (await carRepository.GetByIdAsync(formModel.CarId) is null)
        {
            ModelState.AddModelError(nameof(formModel.CarId), "Car not found.");
            return View("Form", BuildFormModel(formModel));
        }

        await repository.AddAsync(new FuelExpense
        {
            FuelExpenseDate = formModel.FuelExpenseDate,
            Liters = formModel.Liters,
            PricePerLiter = formModel.PricePerLiter,
            Kilometars = formModel.Kilometars,
            CarId = formModel.CarId
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var fuelExpense = await repository.GetByIdAsync(id);
        return fuelExpense is null ? NotFound() : View("Form", await BuildFormModel(fuelExpense));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FuelExpenseFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        if (await carRepository.GetByIdAsync(formModel.CarId) is null)
        {
            ModelState.AddModelError(nameof(formModel.CarId), "Car not found.");
            return View("Form", BuildFormModel(formModel));
        }

        var fuelExpense = new FuelExpense
        {
            Id = formModel.Id,
            FuelExpenseDate = formModel.FuelExpenseDate,
            Liters = formModel.Liters,
            PricePerLiter = formModel.PricePerLiter,
            Kilometars = formModel.Kilometars,
            CarId = formModel.CarId
        };

        if (!await repository.UpdateAsync(fuelExpense))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var fuelExpense = await repository.GetByIdAsync(id);
        return fuelExpense is null ? NotFound() : View(fuelExpense);
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

    private async Task<FuelExpenseFormViewModel> BuildFormModel(FuelExpense? fuelExpense = null)
    {
        return new FuelExpenseFormViewModel
        {
            Id = fuelExpense?.Id ?? 0,
            FuelExpenseDate = fuelExpense?.FuelExpenseDate ?? DateTime.Today,
            Liters = fuelExpense?.Liters ?? 0,
            PricePerLiter = fuelExpense?.PricePerLiter ?? 0,
            Kilometars = fuelExpense?.Kilometars ?? 0,
            CarId = fuelExpense?.CarId ?? 0,
            CarOptions = (await carRepository.GetAllAsync())
                .OrderBy(car => car.Brand)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
                .ToList()
        };
    }

    private async Task<FuelExpenseFormViewModel> BuildFormModel(FuelExpenseFormViewModel formModel)
    {
        formModel.CarOptions = (await carRepository.GetAllAsync())
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
            .ToList();
        return formModel;
    }
}


