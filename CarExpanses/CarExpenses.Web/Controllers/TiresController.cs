using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Authorize]
[Route("gume")]
public class TiresController(ITireRepository repository) : Controller
{
    [Route("[action]")]
    public async Task<IActionResult> Index() => View(await repository.GetAllAsync());

    [HttpGet]
    [Route("pretraga")]
    public async Task<IActionResult> Search(string? query)
    {
        var tires = await repository.GetListAsync(new TireFilter { Search = query });
        return PartialView("_TireList", tires);
    }

    [Route("detalji/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var tire = await repository.GetByIdAsync(id);
        return tire is null ? NotFound() : View(tire);
    }

    [HttpGet]
    [Route("novi")]
    public IActionResult Create() => View("Form", new TireFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("novi")]
    public async Task<IActionResult> Create(TireFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", formModel);
        }

        await repository.AddAsync(new Tire
        {
            Brand = formModel.Brand,
            Model = formModel.Model,
            Season = formModel.Season,
            Price = formModel.Price
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("uredi/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var tire = await repository.GetByIdAsync(id);
        return tire is null ? NotFound() : View("Form", new TireFormViewModel
        {
            Id = tire.Id,
            Brand = tire.Brand,
            Model = tire.Model,
            Season = tire.Season,
            Price = tire.Price
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("uredi/{id}")]
    public async Task<IActionResult> Edit(int id, TireFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", formModel);
        }

        var tire = new Tire
        {
            Id = formModel.Id,
            Brand = formModel.Brand,
            Model = formModel.Model,
            Season = formModel.Season,
            Price = formModel.Price
        };

        if (!await repository.UpdateAsync(tire))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("obrisi/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tire = await repository.GetByIdAsync(id);
        return tire is null ? NotFound() : View(tire);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("obrisi/{id}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await repository.DeleteAsync(id))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }
}


