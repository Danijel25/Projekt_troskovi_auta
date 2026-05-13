using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

public class ExpensesController(IExpenseRepository repository, IExpenseCategoryRepository categoryRepository, CarExpesesDbContext dbContext) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

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

        dbContext.Expenses.Add(new Expense
        {
            Description = formModel.Description,
            Amount = formModel.Amount,
            Date = formModel.Date,
            CategoryId = formModel.CategoryId,
            Category = dbContext.ExpenseCategories.First(category => category.Id == formModel.CategoryId)
        });

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var expense = dbContext.Expenses.AsNoTracking().FirstOrDefault(item => item.Id == id);
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

        var expense = dbContext.Expenses.FirstOrDefault(item => item.Id == id);
        if (expense is null)
        {
            return NotFound();
        }

        expense.Description = formModel.Description;
        expense.Amount = formModel.Amount;
        expense.Date = formModel.Date;
        expense.CategoryId = formModel.CategoryId;
        expense.Category = dbContext.ExpenseCategories.First(category => category.Id == formModel.CategoryId);

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var expense = dbContext.Expenses.Include(item => item.Category).FirstOrDefault(item => item.Id == id);
        return expense is null ? NotFound() : View(expense);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var expense = dbContext.Expenses.Include(item => item.Category).FirstOrDefault(item => item.Id == id);
        if (expense is null)
        {
            return NotFound();
        }

        dbContext.Expenses.Remove(expense);
        dbContext.SaveChanges();
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


