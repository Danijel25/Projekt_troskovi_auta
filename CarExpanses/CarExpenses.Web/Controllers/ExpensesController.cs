using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

public class ExpensesController(IExpenseRepository repository, IExpenseCategoryRepository categoryRepository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    [HttpGet]
    public IActionResult Search(string? query)
    {
        var expenses = repository.GetAll();

        if (string.IsNullOrWhiteSpace(query))
        {
            return PartialView("_ExpenseList", expenses);
        }

        var term = query.Trim();
        var filtered = expenses
            .Where(expense =>
                expense.Description.Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.Amount.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.Date.ToString("yyyy-MM-dd").Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.Date.ToString("MM/dd").Contains(term, StringComparison.OrdinalIgnoreCase)
                || expense.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || (expense.Category?.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        return PartialView("_ExpenseList", filtered);
    }

    public IActionResult Details(int id)
    {
        var expense = repository.GetById(id);
        return expense is null ? NotFound() : View(expense);
    }

    [HttpGet]
    public IActionResult Create() => View("Form", BuildFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ExpenseFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        repository.Add(new Expense
        {
            Description = formModel.Description,
            Amount = formModel.Amount,
            Date = formModel.Date,
            CategoryId = formModel.CategoryId,
            Category = null!
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var expense = repository.GetById(id);
        return expense is null ? NotFound() : View("Form", BuildFormModel(expense));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ExpenseFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        var expense = new Expense
        {
            Id = formModel.Id,
            Description = formModel.Description,
            Amount = formModel.Amount,
            Date = formModel.Date,
            CategoryId = formModel.CategoryId,
            Category = null!
        };

        if (!repository.Update(expense))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var expense = repository.GetById(id);
        return expense is null ? NotFound() : View(expense);
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

    private ExpenseFormViewModel BuildFormModel(Expense? expense = null)
    {
        return new ExpenseFormViewModel
        {
            Id = expense?.Id ?? 0,
            Description = expense?.Description ?? string.Empty,
            Amount = expense?.Amount ?? 0,
            Date = expense?.Date ?? DateTime.Today,
            CategoryId = expense?.CategoryId ?? 0,
            CategoryOptions = categoryRepository.GetAll()
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
                .ToList()
        };
    }

    private ExpenseFormViewModel BuildFormModel(ExpenseFormViewModel formModel)
    {
        formModel.CategoryOptions = categoryRepository.GetAll()
            .OrderBy(category => category.Name)
            .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
            .ToList();
        return formModel;
    }
}


