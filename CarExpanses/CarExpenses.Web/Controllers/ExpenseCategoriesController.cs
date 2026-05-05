using CarExpenses.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

public class ExpenseCategoriesController(IExpenseCategoryRepository repository) : Controller
{
	public IActionResult Index() => View(repository.GetAll());

	public IActionResult Details(int id)
	{
		var category = repository.GetById(id);
		return category is null ? NotFound() : View(category);
	}
}
