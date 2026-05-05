using CarExpenses.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Route("troskovi-goriva/[action]")]
public class FuelExpensesController(IFuelExpenseRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var fuelExpense = repository.GetById(id);
        return fuelExpense is null ? NotFound() : View(fuelExpense);
    }
}


