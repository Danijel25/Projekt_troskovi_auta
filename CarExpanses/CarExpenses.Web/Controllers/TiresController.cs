using CarExpenses.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Route("gume")]
public class TiresController(ITireRepository repository) : Controller
{
    [Route("[action]")]
    public IActionResult Index() => View(repository.GetAll());

    [Route("detalji/{id}")]
    public IActionResult Details(int id)
    {
        var tire = repository.GetById(id);
        return tire is null ? NotFound() : View(tire);
    }
}


