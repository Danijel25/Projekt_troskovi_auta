using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class FuelExpenseRepository(CarExpesesDbContext dbContext) : IFuelExpenseRepository
{
    public IReadOnlyList<FuelExpense> GetAll() => dbContext.FuelExpenses
        .Include(fuelExpense => fuelExpense.Car)
        .AsNoTracking()
        .OrderByDescending(fuelExpense => fuelExpense.FuelExpenseDate)
        .ToList();

    public FuelExpense? GetById(int id) => dbContext.FuelExpenses
        .Include(fuelExpense => fuelExpense.Car)
        .AsNoTracking()
        .FirstOrDefault(fuelExpense => fuelExpense.Id == id);
}