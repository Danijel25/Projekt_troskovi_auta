using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/service-records")]
public sealed class ServiceRecordsApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly IServiceRecordRepository serviceRecordRepository;

    public ServiceRecordsApiController(CarExpesesDbContext dbContext, IServiceRecordRepository serviceRecordRepository)
    {
        this.dbContext = dbContext;
        this.serviceRecordRepository = serviceRecordRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceRecordDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? carId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var serviceRecords = await serviceRecordRepository
            .Query(new ServiceRecordFilter
            {
                Search = search,
                CarId = carId,
                FromDate = fromDate,
                ToDate = toDate
            })
            .ToListAsync();
        var result = serviceRecords.Select(DtoMapping.ToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceRecordDto>> GetById(int id)
    {
        var serviceRecord = await dbContext.ServiceRecords
            .Include(item => item.Car)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (serviceRecord is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDto(serviceRecord));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceRecordDto>> Create(ServiceRecordCreateDto dto)
    {
        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        var serviceRecord = new ServiceRecord
        {
            ServiceType = dto.ServiceType,
            Description = dto.Description,
            Cost = dto.Cost,
            ServiceDate = dto.ServiceDate,
            Mileage = dto.Mileage,
            CarId = dto.CarId
        };

        dbContext.ServiceRecords.Add(serviceRecord);
        await dbContext.SaveChangesAsync();

        var created = await dbContext.ServiceRecords
            .Include(item => item.Car)
            .AsNoTracking()
            .FirstAsync(item => item.Id == serviceRecord.Id);

        return CreatedAtAction(nameof(GetById), new { id = serviceRecord.Id }, DtoMapping.ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ServiceRecordUpdateDto dto)
    {
        var serviceRecord = await dbContext.ServiceRecords.FirstOrDefaultAsync(item => item.Id == id);
        if (serviceRecord is null)
        {
            return NotFound();
        }

        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        serviceRecord.ServiceType = dto.ServiceType;
        serviceRecord.Description = dto.Description;
        serviceRecord.Cost = dto.Cost;
        serviceRecord.ServiceDate = dto.ServiceDate;
        serviceRecord.Mileage = dto.Mileage;
        serviceRecord.CarId = dto.CarId;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceRecord = await dbContext.ServiceRecords.FirstOrDefaultAsync(item => item.Id == id);
        if (serviceRecord is null)
        {
            return NotFound();
        }

        dbContext.ServiceRecords.Remove(serviceRecord);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
