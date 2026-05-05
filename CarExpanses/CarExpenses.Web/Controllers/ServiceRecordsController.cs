using CarExpenses.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

public class ServiceRecordsController(IServiceRecordRepository repository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    public IActionResult Details(int id)
    {
        var serviceRecord = repository.GetById(id);
        return serviceRecord is null ? NotFound() : View(serviceRecord);
    }
}


