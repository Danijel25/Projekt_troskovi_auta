using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Authorize]
public class ExpenseCategoriesController(IExpenseCategoryRepository repository, Serilog.ILogger logger) : Controller
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
		logger.Information("Attempting to create expense category {CategoryName}", formModel.Name);
		logger.Debug("Received form data: {@FormModel}", formModel);
		try
		{
			if (!ModelState.IsValid)
			{
				return View("Form", formModel);
			}	

			await repository.AddAsync(new ExpenseCategory { Name = formModel.Name });
			return RedirectToAction(nameof(Index));
		} 
		catch(Exception ex)
		{
			logger.Error(ex, "Error creating expense category");
			ModelState.AddModelError(string.Empty, "An error occurred while creating the expense category. Please try again.");
			return View("Form", formModel);
		}
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
