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
    public IActionResult Index() => View(repository.GetAll());

    [HttpGet]
    public IActionResult Search(string? query)
    {
        var fuelExpenses = repository.GetAll();

        if (string.IsNullOrWhiteSpace(query))
        {
            return PartialView("_FuelExpenseList", fuelExpenses);
        }

        var term = query.Trim();
        var filtered = fuelExpenses
            .Where(expense =>
                expense.CarId.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.TotalCost.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.Liters.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.PricePerLiter.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.Kilometars.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.FuelExpenseDate.ToString("yyyy-MM-dd").Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || (expense.Car?.Brand?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (expense.Car?.Model?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        return PartialView("_FuelExpenseList", filtered);
    }

    public IActionResult Details(int id)
    {
        var fuelExpense = repository.GetById(id);
        return fuelExpense is null ? NotFound() : View(fuelExpense);
    }

    [HttpGet]
    public IActionResult Create() => View("Form", BuildFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(FuelExpenseFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        if (carRepository.GetById(formModel.CarId) is null)
        {
            ModelState.AddModelError(nameof(formModel.CarId), "Car not found.");
            return View("Form", BuildFormModel(formModel));
        }

        repository.Add(new FuelExpense
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
    public IActionResult Edit(int id)
    {
        var fuelExpense = repository.GetById(id);
        return fuelExpense is null ? NotFound() : View("Form", BuildFormModel(fuelExpense));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, FuelExpenseFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        if (carRepository.GetById(formModel.CarId) is null)
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

        if (!repository.Update(fuelExpense))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var fuelExpense = repository.GetById(id);
        return fuelExpense is null ? NotFound() : View(fuelExpense);
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

    private FuelExpenseFormViewModel BuildFormModel(FuelExpense? fuelExpense = null)
    {
        return new FuelExpenseFormViewModel
        {
            Id = fuelExpense?.Id ?? 0,
            FuelExpenseDate = fuelExpense?.FuelExpenseDate ?? DateTime.Today,
            Liters = fuelExpense?.Liters ?? 0,
            PricePerLiter = fuelExpense?.PricePerLiter ?? 0,
            Kilometars = fuelExpense?.Kilometars ?? 0,
            CarId = fuelExpense?.CarId ?? 0,
            CarOptions = carRepository.GetAll()
                .OrderBy(car => car.Brand)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
                .ToList()
        };
    }

    private FuelExpenseFormViewModel BuildFormModel(FuelExpenseFormViewModel formModel)
    {
        formModel.CarOptions = carRepository.GetAll()
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
            .ToList();
        return formModel;
    }
}


