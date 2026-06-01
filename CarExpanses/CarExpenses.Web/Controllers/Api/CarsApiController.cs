using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/cars")]
public sealed class CarsApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly ICurrentUserService currentUserService;
    private readonly ICarRepository carRepository;

    public CarsApiController(
        CarExpesesDbContext dbContext,
        ICurrentUserService currentUserService,
        ICarRepository carRepository)
    {
        this.dbContext = dbContext;
        this.currentUserService = currentUserService;
        this.carRepository = carRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarListItemDto>>> GetAll(
        [FromQuery] CarFilter filter)
    {
        var cars = await carRepository
            .GetListAsync(filter);
        var result = cars.Select(DtoMapping.ToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarDetailDto>> GetById(int id)
    {
        var car = await carRepository.GetByIdAsync(id);
        if (car is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDetailDto(car));
    }

    [HttpPost]
    public async Task<ActionResult<CarDetailDto>> Create(CarCreateDto dto)
    {
        var userId = dto.UserId;
        if (!User.IsInRole(AppRoles.Admin))
        {
            if (!currentUserService.UserId.HasValue)
            {
                return Forbid();
            }

            userId = currentUserService.UserId.Value;
        }

        if (!await dbContext.Users.AnyAsync(user => user.Id == userId))
        {
            ModelState.AddModelError(nameof(dto.UserId), "User not found.");
            return ValidationProblem(ModelState);
        }

        var car = new Car
        {
            UserId = userId,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            EngineVolume = dto.EngineVolume,
            CurrentMilage = dto.CurrentMilage,
            PurchasePrice = dto.PurchasePrice,
            PurchaseDate = dto.PurchaseDate,
            FuelType = dto.FuelType
        };

        var carId = await carRepository.AddAsync(car);

        var created = await carRepository.GetByIdAsync(car.Id);
        if (created is null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetById), new { id = car.Id }, DtoMapping.ToDetailDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CarUpdateDto dto)
    {
        var car = await carRepository.GetByIdAsync(id);
        if (car is null)
        {
            return NotFound();
        }

        if (User.IsInRole(AppRoles.Admin))
        {
            if (!await dbContext.Users.AnyAsync(user => user.Id == dto.UserId))
            {
                ModelState.AddModelError(nameof(dto.UserId), "User not found.");
                return ValidationProblem(ModelState);
            }

            car.UserId = dto.UserId;
        }
        await carRepository.UpdateAsync(new ()
        {
            Id = id,
            UserId = car.UserId,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            EngineVolume = dto.EngineVolume,
            CurrentMilage = dto.CurrentMilage,
            PurchasePrice = dto.PurchasePrice,
            PurchaseDate = dto.PurchaseDate,
            FuelType = dto.FuelType
        });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await carRepository.GetByIdAsync(id);

        if (car is null)
        {
            return NotFound();
        }

        await carRepository.DeleteAsync(id);
        return NoContent();
    }

    private Task<Car?> GetCarDetailsAsync(int id)
    {
        return dbContext.Cars
            .Include(item => item.User)
            .Include(item => item.FuelExpenses)
            .Include(item => item.ServiceRecords)
            .Include(item => item.Insurances)
            .Include(item => item.CarTires)!
                .ThenInclude(carTire => carTire.Tire)
            .Include(item => item.Expenses)!
                .ThenInclude(expense => expense.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);
    }
}
