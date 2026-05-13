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

    public void Add(ExpenseCategory category)
    {
        dbContext.ExpenseCategories.Add(category);
        dbContext.SaveChanges();
    }

    public bool Update(ExpenseCategory category)
    {
        var existing = dbContext.ExpenseCategories.FirstOrDefault(item => item.Id == category.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Name = category.Name;
        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var category = dbContext.ExpenseCategories
            .Include(item => item.Expenses)
            .FirstOrDefault(item => item.Id == id);

        if (category is null)
        {
            return false;
        }

        dbContext.ExpenseCategories.Remove(category);
        dbContext.SaveChanges();
        return true;
    }
}