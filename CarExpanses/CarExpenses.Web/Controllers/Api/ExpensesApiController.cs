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
        [FromQuery] ExpenseFilter filter)
    {
        var expense = (await expenseRepository
                    .GetListAsync(filter))
                    .Select(DtoMapping.ToSummaryDto);        

        return Ok(expense);        
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseDetailDto>> GetById(int id)
    {
        var entity = await expenseRepository.GetByIdAsync(id);
        if(entity is null)
        {
            return NotFound();
        }
        var dto = DtoMapping.ToDto(entity);
        return Ok(dto);
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

        var expenseId = await expenseRepository.AddAsync(expense);
        
        var created = await expenseRepository.GetByIdAsync(expenseId);
        var createdDto = DtoMapping.ToDto(created);

        return CreatedAtAction(nameof(Create), new { id = expense.Id }, createdDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ExpenseUpdateDto dto)
    {
        var expense = await expenseRepository.GetByIdAsync(id);
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

        await expenseRepository.UpdateAsync(new()
        {
            Id = id,
            Description = dto.Description,
            Amount = dto.Amount,
            Date = dto.Date,
            CategoryId = dto.CategoryId,
            CarId = dto.CarId
        });
        
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await expenseRepository.GetByIdAsync(id);
        if (expense is null)
        {
            return NotFound();
        }

        await expenseRepository.DeleteAsync(id);
        return NoContent();
    }    
}
