using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

[Route("troskovi-goriva/[action]")]
public class FuelExpensesController(IFuelExpenseRepository repository, ICarRepository carRepository, CarExpesesDbContext dbContext) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

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

        dbContext.FuelExpenses.Add(new FuelExpense
        {
            FuelExpenseDate = formModel.FuelExpenseDate,
            Liters = formModel.Liters,
            PricePerLiter = formModel.PricePerLiter,
            Kilometars = formModel.Kilometars,
            CarId = formModel.CarId
        });

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var fuelExpense = dbContext.FuelExpenses.AsNoTracking().FirstOrDefault(item => item.Id == id);
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

        var fuelExpense = dbContext.FuelExpenses.FirstOrDefault(item => item.Id == id);
        if (fuelExpense is null)
        {
            return NotFound();
        }

        fuelExpense.FuelExpenseDate = formModel.FuelExpenseDate;
        fuelExpense.Liters = formModel.Liters;
        fuelExpense.PricePerLiter = formModel.PricePerLiter;
        fuelExpense.Kilometars = formModel.Kilometars;
        fuelExpense.CarId = formModel.CarId;

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var fuelExpense = dbContext.FuelExpenses.Include(item => item.Car).FirstOrDefault(item => item.Id == id);
        return fuelExpense is null ? NotFound() : View(fuelExpense);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var fuelExpense = dbContext.FuelExpenses.Include(item => item.Car).FirstOrDefault(item => item.Id == id);
        if (fuelExpense is null)
        {
            return NotFound();
        }

        dbContext.FuelExpenses.Remove(fuelExpense);
        dbContext.SaveChanges();
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


