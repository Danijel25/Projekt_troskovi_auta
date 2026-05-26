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
[Route("api/expenses")]
public sealed class ExpensesApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly IExpenseRepository expenseRepository;

    public ExpensesApiController(CarExpesesDbContext dbContext, IExpenseRepository expenseRepository)
    {
        this.dbContext = dbContext;
        this.expenseRepository = expenseRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseListItemDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] int? carId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount)
    {
        var query = expenseRepository.Query(new ExpenseFilter
        {
            Search = search,
            CategoryId = categoryId,
            CarId = carId,
            FromDate = fromDate,
            ToDate = toDate,
            MinAmount = minAmount,
            MaxAmount = maxAmount
        });

        var items = await query
            .Select(item => new ExpenseListItemDto
            {
                Id = item.Id,
                Description = item.Description,
                Amount = item.Amount,
                Date = item.Date,
                CategoryId = item.CategoryId,
                Category = item.Category == null ? null : new ExpenseCategoryDto
                {
                    Id = item.Category.Id,
                    Name = item.Category.Name
                },
                CarId = item.CarId
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseDetailDto>> GetById(int id)
    {
        var dto = await BuildExpenseDetailAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDetailDto>> Create(ExpenseCreateDto dto)
    {
        if (!await dbContext.ExpenseCategories.AnyAsync(category => category.Id == dto.CategoryId))
        {
            ModelState.AddModelError(nameof(dto.CategoryId), "Category not found.");
            return ValidationProblem(ModelState);
        }

        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        var expense = new Expense
        {
            Description = dto.Description,
            Amount = dto.Amount,
            Date = dto.Date,
            CategoryId = dto.CategoryId,
            Category = null!,
            CarId = dto.CarId
        };

        dbContext.Expenses.Add(expense);

        await dbContext.SaveChangesAsync();

        var created = await BuildExpenseDetailAsync(expense.Id);
        if (created is null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ExpenseUpdateDto dto)
    {
        var expense = await dbContext.Expenses.FirstOrDefaultAsync(item => item.Id == id);
        if (expense is null)
        {
            return NotFound();
        }

        if (!await dbContext.ExpenseCategories.AnyAsync(category => category.Id == dto.CategoryId))
        {
            ModelState.AddModelError(nameof(dto.CategoryId), "Category not found.");
            return ValidationProblem(ModelState);
        }

        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        expense.Description = dto.Description;
        expense.Amount = dto.Amount;
        expense.Date = dto.Date;
        expense.CategoryId = dto.CategoryId;
        expense.Category = null!;
        expense.CarId = dto.CarId;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await dbContext.Expenses.FirstOrDefaultAsync(item => item.Id == id);
        if (expense is null)
        {
            return NotFound();
        }

        dbContext.Expenses.Remove(expense);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    private async Task<ExpenseDetailDto?> BuildExpenseDetailAsync(int id)
    {
        var expense = await dbContext.Expenses
            .Include(item => item.Category)
            .Include(item => item.Car)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (expense is null)
        {
            return null;
        }

        return new ExpenseDetailDto
        {
            Id = expense.Id,
            Description = expense.Description,
            Amount = expense.Amount,
            Date = expense.Date,
            CategoryId = expense.CategoryId,
            Category = expense.Category is null ? null : DtoMapping.ToDto(expense.Category),
            CarId = expense.CarId,
            Car = expense.Car is null ? null : DtoMapping.ToSummaryDto(expense.Car)
        };
    }
}
