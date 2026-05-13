using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

public class ExpenseCategoriesController(IExpenseCategoryRepository repository) : Controller
{
	public IActionResult Index() => View(repository.GetAll());

	[HttpGet]
	public IActionResult Search(string? query)
	{
		var categories = repository.GetAll();
		if (string.IsNullOrWhiteSpace(query))
		{
			return PartialView("_ExpenseCategoryList", categories);
		}

		var term = query.Trim();
		var filtered = categories
			.Where(category =>
				category.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
				|| category.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase))
			.ToList();

		return PartialView("_ExpenseCategoryList", filtered);
	}

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

		repository.Add(new ExpenseCategory { Name = formModel.Name });
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public IActionResult Edit(int id)
	{
		var category = repository.GetById(id);
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

		var category = new ExpenseCategory
		{
			Id = formModel.Id,
			Name = formModel.Name
		};

		if (!repository.Update(category))
		{
			return NotFound();
		}
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public IActionResult Delete(int id)
	{
		var category = repository.GetById(id);
		return category is null ? NotFound() : View(category);
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
}
