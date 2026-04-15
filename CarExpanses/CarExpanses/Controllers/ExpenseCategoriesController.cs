using CarExpanses.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpanses.Controllers;

public class ExpenseCategoriesController(ExpenseCategoryMockRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var category = repository.GetById(id);
        return category is null ? NotFound() : View(category);
    }
}
