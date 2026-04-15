using CarExpanses.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpanses.Controllers;

public class InsurancesController(InsuranceMockRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var insurance = repository.GetById(id);
        return insurance is null ? NotFound() : View(insurance);
    }
}
