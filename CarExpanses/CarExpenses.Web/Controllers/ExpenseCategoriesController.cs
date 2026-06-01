using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Authorize]
public class ExpenseCategoriesController(IExpenseCategoryRepository repository) : Controller
{
	public async Task<IActionResult> Index() => View(await repository.GetAllAsync());

	[HttpGet]
	public async Task<IActionResult> Search(string? query)
	{
		var categories = await repository.GetListAsync(new ExpenseCategoryFilter { Search = query });
		return PartialView("_ExpenseCategoryList", categories);
	}

	public async Task<IActionResult> Details(int id)
	{
		var category = await repository.GetByIdAsync(id);
		return category is null ? NotFound() : View(category);
	}

	[Authorize(Roles = AppRoles.Admin)]
	[HttpGet]
	public IActionResult Create() => View("Form", new ExpenseCategoryFormViewModel());

	[Authorize(Roles = AppRoles.Admin)]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(ExpenseCategoryFormViewModel formModel)
	{
		if (!ModelState.IsValid)
		{
			return View("Form", formModel);
		}

		await repository.AddAsync(new ExpenseCategory { Name = formModel.Name });
		return RedirectToAction(nameof(Index));
	}

	[Authorize(Roles = AppRoles.Admin)]
	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var category = await repository.GetByIdAsync(id);
		return category is null ? NotFound() : View("Form", new ExpenseCategoryFormViewModel
		{
			Id = category.Id,
			Name = category.Name
		});
	}

	[Authorize(Roles = AppRoles.Admin)]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, ExpenseCategoryFormViewModel formModel)
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

		if (!await repository.UpdateAsync(category))
		{
			return NotFound();
		}
		return RedirectToAction(nameof(Index));
	}

	[Authorize(Roles = AppRoles.Admin)]
	[HttpGet]
	public async Task<IActionResult> Delete(int id)
	{
		var category = await repository.GetByIdAsync(id);
		return category is null ? NotFound() : View(category);
	}

	[Authorize(Roles = AppRoles.Admin)]
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
}
