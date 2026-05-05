using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class ExpenseRepository(CarExpesesDbContext dbContext) : IExpenseRepository
{
    public IReadOnlyList<Expense> GetAll() => dbContext.Expenses
        .Include(expense => expense.Category)
        .AsNoTracking()
        .OrderByDescending(expense => expense.Date)
        .ToList();

    public Expense? GetById(int id) => dbContext.Expenses
        .Include(expense => expense.Category)
        .AsNoTracking()
        .FirstOrDefault(expense => expense.Id == id);
}