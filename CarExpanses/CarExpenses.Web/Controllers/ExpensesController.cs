using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

[Authorize]
public class ExpensesController(IExpenseRepository repository, IExpenseCategoryRepository categoryRepository, ICarRepository carRepository) : Controller
{
    public async Task<IActionResult> Index() => View(await repository.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        var expenses = await repository.GetListAsync(new ExpenseFilter { Search = query });
        return PartialView("_ExpenseList", expenses);
    }

    public async Task<IActionResult> Details(int id)
    {
        var expense = await repository.GetByIdAsync(id);
        return expense is null ? NotFound() : View(expense);
    }

    [HttpGet]
    public async Task<IActionResult> Create() => View("Form", await BuildFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseFormViewModel formModel)
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

        await repository.AddAsync(new Expense
        {
            Description = formModel.Description,
            Amount = formModel.Amount,
            Date = formModel.Date,
            CategoryId = formModel.CategoryId,
            Category = null!,
            CarId = formModel.CarId
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var expense = await repository.GetByIdAsync(id);
        return expense is null ? NotFound() : View("Form", await BuildFormModel(expense));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExpenseFormViewModel formModel)
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

        var expense = new Expense
        {
            Id = formModel.Id,
            Description = formModel.Description,
            Amount = formModel.Amount,
            Date = formModel.Date,
            CategoryId = formModel.CategoryId,
            Category = null!,
            CarId = formModel.CarId
        };

        if (!await repository.UpdateAsync(expense))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await repository.GetByIdAsync(id);
        return expense is null ? NotFound() : View(expense);
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

    private async Task<ExpenseFormViewModel> BuildFormModel(Expense? expense = null)
    {
        return new ExpenseFormViewModel
        {
            Id = expense?.Id ?? 0,
            Description = expense?.Description ?? string.Empty,
            Amount = expense?.Amount ?? 0,
            Date = expense?.Date ?? DateTime.Today,
            CategoryId = expense?.CategoryId ?? 0,
            CarId = expense?.CarId ?? 0,
            CategoryOptions = (await categoryRepository.GetAllAsync())
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
                .ToList(),
            CarOptions = (await carRepository.GetAllAsync())
                .OrderBy(car => car.Brand)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
                .ToList()
        };
    }

    private async Task<ExpenseFormViewModel> BuildFormModel(ExpenseFormViewModel formModel)
    {
        formModel.CategoryOptions = (await categoryRepository.GetAllAsync())
            .OrderBy(category => category.Name)
            .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
            .ToList();
        formModel.CarOptions = (await carRepository.GetAllAsync())
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
            .ToList();
        return formModel;
    }
}


