using CarExpanses.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpanses.Controllers;

public class FuelExpensesController(FuelExpenseMockRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var fuelExpense = repository.GetById(id);
        return fuelExpense is null ? NotFound() : View(fuelExpense);
    }
}
