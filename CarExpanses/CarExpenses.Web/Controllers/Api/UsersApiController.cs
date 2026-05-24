using CarExpenses.DAL;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers.Api;

[ApiController]
[Route("api/users")]
public sealed class UsersApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;

    public UsersApiController(CarExpesesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserSummaryDto>>> GetAll([FromQuery] string? search)
    {
        var query = dbContext.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(user => user.Username.Contains(term) || user.Email.Contains(term));
        }

        var users = await query.OrderBy(user => user.Id).ToListAsync();
        var result = users.Select(DtoMapping.ToSummaryDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDetailDto>> GetById(int id)
    {
        var user = await dbContext.Users
            .Include(item => item.Cars)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDetailDto(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserDetailDto>> Create(UserCreateDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var created = await dbContext.Users
            .Include(item => item.Cars)
            .AsNoTracking()
            .FirstAsync(item => item.Id == user.Id);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, DtoMapping.ToDetailDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UserUpdateDto dto)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(item => item.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.Password = dto.Password;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await dbContext.Users
            .Include(item => item.Cars)!
                .ThenInclude(car => car.FuelExpenses)
            .Include(item => item.Cars)!
                .ThenInclude(car => car.ServiceRecords)
            .Include(item => item.Cars)!
                .ThenInclude(car => car.Insurances)
            .Include(item => item.Cars)!
                .ThenInclude(car => car.CarTires)!
                    .ThenInclude(carTire => carTire.Tire)
            .Include(item => item.Cars)!
                .ThenInclude(car => car.Expenses)!
                    .ThenInclude(expense => expense.Category)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
