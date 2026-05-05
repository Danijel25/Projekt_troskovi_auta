using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class ExpenseCategoryRepository(CarExpesesDbContext dbContext) : IExpenseCategoryRepository
{
    public IReadOnlyList<ExpenseCategory> GetAll() => dbContext.ExpenseCategories
        .Include(category => category.Expenses)
        .AsNoTracking()
        .OrderBy(category => category.Id)
        .ToList();

    public ExpenseCategory? GetById(int id) => dbContext.ExpenseCategories
        .Include(category => category.Expenses)
        .AsNoTracking()
        .FirstOrDefault(category => category.Id == id);
}