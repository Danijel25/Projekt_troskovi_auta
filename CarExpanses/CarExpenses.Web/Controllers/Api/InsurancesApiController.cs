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
[Route("api/insurances")]
public sealed class InsurancesApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly IInsuranceRepository insuranceRepository;

    public InsurancesApiController(CarExpesesDbContext dbContext, IInsuranceRepository insuranceRepository)
    {
        this.dbContext = dbContext;
        this.insuranceRepository = insuranceRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InsuranceDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? carId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var insurances = await insuranceRepository
            .Query(new InsuranceFilter
            {
                Search = search,
                CarId = carId,
                FromDate = fromDate,
                ToDate = toDate
            })
            .ToListAsync();
        var result = insurances.Select(DtoMapping.ToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InsuranceDto>> GetById(int id)
    {
        var insurance = await dbContext.Insurances
            .Include(item => item.Car)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (insurance is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDto(insurance));
    }

    [HttpPost]
    public async Task<ActionResult<InsuranceDto>> Create(InsuranceCreateDto dto)
    {
        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        if (dto.EndDate < dto.StartDate)
        {
            ModelState.AddModelError(nameof(dto.EndDate), "EndDate must be on or after StartDate.");
            return ValidationProblem(ModelState);
        }

        var insurance = new Insurance
        {
            Company = dto.Company,
            InsuranceType = dto.InsuranceType,
            Price = dto.Price,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CarId = dto.CarId
        };

        dbContext.Insurances.Add(insurance);
        await dbContext.SaveChangesAsync();

        var created = await dbContext.Insurances
            .Include(item => item.Car)
            .AsNoTracking()
            .FirstAsync(item => item.Id == insurance.Id);

        return CreatedAtAction(nameof(GetById), new { id = insurance.Id }, DtoMapping.ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, InsuranceUpdateDto dto)
    {
        var insurance = await dbContext.Insurances.FirstOrDefaultAsync(item => item.Id == id);
        if (insurance is null)
        {
            return NotFound();
        }

        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        if (dto.EndDate < dto.StartDate)
        {
            ModelState.AddModelError(nameof(dto.EndDate), "EndDate must be on or after StartDate.");
            return ValidationProblem(ModelState);
        }

        insurance.Company = dto.Company;
        insurance.InsuranceType = dto.InsuranceType;
        insurance.Price = dto.Price;
        insurance.StartDate = dto.StartDate;
        insurance.EndDate = dto.EndDate;
        insurance.CarId = dto.CarId;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var insurance = await dbContext.Insurances.FirstOrDefaultAsync(item => item.Id == id);
        if (insurance is null)
        {
            return NotFound();
        }

        dbContext.Insurances.Remove(insurance);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
