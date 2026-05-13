using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

public class ExpenseCategoriesController(IExpenseCategoryRepository repository, CarExpesesDbContext dbContext) : Controller
{
	public IActionResult Index() => View(repository.GetAll());

	public IActionResult Details(int id)
	{
		var category = repository.GetById(id);
		return category is null ? NotFound() : View(category);
	}

	[HttpGet]
	public IActionResult Create() => View("Form", new ExpenseCategoryFormViewModel());

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Create(ExpenseCategoryFormViewModel formModel)
	{
		if (!ModelState.IsValid)
		{
			return View("Form", formModel);
		}

		dbContext.ExpenseCategories.Add(new ExpenseCategory { Name = formModel.Name });
		dbContext.SaveChanges();
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public IActionResult Edit(int id)
	{
		var category = dbContext.ExpenseCategories.AsNoTracking().FirstOrDefault(category => category.Id == id);
		return category is null ? NotFound() : View("Form", new ExpenseCategoryFormViewModel
		{
			Id = category.Id,
			Name = category.Name
		});
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Edit(int id, ExpenseCategoryFormViewModel formModel)
	{
		if (id != formModel.Id)
		{
			return BadRequest();
		}

		if (!ModelState.IsValid)
		{
			return View("Form", formModel);
		}

		var category = dbContext.ExpenseCategories.FirstOrDefault(category => category.Id == id);
		if (category is null)
		{
			return NotFound();
		}

		category.Name = formModel.Name;
		dbContext.SaveChanges();
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public IActionResult Delete(int id)
	{
		var category = dbContext.ExpenseCategories.Include(category => category.Expenses).FirstOrDefault(category => category.Id == id);
		return category is null ? NotFound() : View(category);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public IActionResult DeleteConfirmed(int id)
	{
		var category = dbContext.ExpenseCategories.Include(category => category.Expenses).FirstOrDefault(category => category.Id == id);
		if (category is null)
		{
			return NotFound();
		}

		dbContext.ExpenseCategories.Remove(category);
		dbContext.SaveChanges();
		return RedirectToAction(nameof(Index));
	}
}
