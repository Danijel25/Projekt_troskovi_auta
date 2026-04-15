using CarExpanses.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpanses.Controllers;

public class ServiceRecordsController(ServiceRecordMockRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var serviceRecord = repository.GetById(id);
        return serviceRecord is null ? NotFound() : View(serviceRecord);
    }
}
