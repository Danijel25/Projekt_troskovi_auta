using CarExpanses.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpanses.Controllers;

public class ExpensesController(ExpenseMockRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var expense = repository.GetById(id);
        return expense is null ? NotFound() : View(expense);
    }
}
