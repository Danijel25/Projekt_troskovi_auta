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

    public void Add(FuelExpense fuelExpense)
    {
        dbContext.FuelExpenses.Add(fuelExpense);
        dbContext.SaveChanges();
    }

    public bool Update(FuelExpense fuelExpense)
    {
        var existing = dbContext.FuelExpenses.FirstOrDefault(item => item.Id == fuelExpense.Id);
        if (existing is null)
        {
            return false;
        }

        existing.FuelExpenseDate = fuelExpense.FuelExpenseDate;
        existing.Liters = fuelExpense.Liters;
        existing.PricePerLiter = fuelExpense.PricePerLiter;
        existing.Kilometars = fuelExpense.Kilometars;
        existing.CarId = fuelExpense.CarId;

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var fuelExpense = dbContext.FuelExpenses.FirstOrDefault(item => item.Id == id);
        if (fuelExpense is null)
        {
            return false;
        }

        dbContext.FuelExpenses.Remove(fuelExpense);
        dbContext.SaveChanges();
        return true;
    }
}