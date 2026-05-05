using CarExpenses.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

public class ExpensesController(IExpenseRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var expense = repository.GetById(id);
        return expense is null ? NotFound() : View(expense);
    }
}


