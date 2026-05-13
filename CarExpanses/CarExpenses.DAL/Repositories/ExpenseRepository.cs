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

    public void Add(Expense expense)
    {
        expense.Category ??= dbContext.ExpenseCategories.First(category => category.Id == expense.CategoryId);
        dbContext.Expenses.Add(expense);
        dbContext.SaveChanges();
    }

    public bool Update(Expense expense)
    {
        var existing = dbContext.Expenses.FirstOrDefault(item => item.Id == expense.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Description = expense.Description;
        existing.Amount = expense.Amount;
        existing.Date = expense.Date;
        existing.CategoryId = expense.CategoryId;
        existing.Category = dbContext.ExpenseCategories.First(category => category.Id == expense.CategoryId);

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var expense = dbContext.Expenses
            .Include(item => item.Category)
            .FirstOrDefault(item => item.Id == id);

        if (expense is null)
        {
            return false;
        }

        dbContext.Expenses.Remove(expense);
        dbContext.SaveChanges();
        return true;
    }
}