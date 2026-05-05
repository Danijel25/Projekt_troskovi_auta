using CarExpenses.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

public class InsurancesController(IInsuranceRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var insurance = repository.GetById(id);
        return insurance is null ? NotFound() : View(insurance);
    }
}


