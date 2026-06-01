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
        [FromQuery] ServiceRecordFilter filter)
    {
        var serviceRecords = await serviceRecordRepository
            .GetListAsync(filter);
        var result = serviceRecords.Select(DtoMapping.ToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceRecordDto>> GetById(int id)
    {
        var serviceRecord = await serviceRecordRepository.GetByIdAsync(id);

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

        var serviceRecordId = await serviceRecordRepository.AddAsync(serviceRecord);

        var created = await serviceRecordRepository.GetByIdAsync(serviceRecordId);

        return CreatedAtAction(nameof(GetById), new { id = serviceRecord.Id }, DtoMapping.ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ServiceRecordUpdateDto dto)
    {
        var serviceRecord = await serviceRecordRepository.GetByIdAsync(id);
        if (serviceRecord is null)
        {
            return NotFound();
        }

        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        await serviceRecordRepository.UpdateAsync(new()
        {
            Id = id,
            ServiceType = dto.ServiceType,
            Description = dto.Description,
            Cost = dto.Cost,
            ServiceDate = dto.ServiceDate,
            Mileage = dto.Mileage,
            CarId = dto.CarId
        });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceRecord = await serviceRecordRepository.GetByIdAsync(id);
        if (serviceRecord is null)
        {
            return NotFound();
        }

        await serviceRecordRepository.DeleteAsync(id);
        return NoContent();
    }
}
