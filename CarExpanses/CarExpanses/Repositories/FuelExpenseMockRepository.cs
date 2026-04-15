using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class FuelExpenseMockRepository
{
    private readonly List<FuelExpense> _fuelExpenses =
    [
        new FuelExpense
        {
            Id = 5001,
            FuelExpenseDate = new DateTime(2026, 2, 15),
            Liters = 47.3m,
            PricePerLiter = 1.49m,
            Kilometars = 123900,
            CarId = 101
        },
        new FuelExpense
        {
            Id = 5002,
            FuelExpenseDate = new DateTime(2026, 2, 21),
            Liters = 35.8m,
            PricePerLiter = 1.53m,
            Kilometars = 67810,
            CarId = 102
        },
        new FuelExpense
        {
            Id = 5003,
            FuelExpenseDate = new DateTime(2026, 3, 1),
            Liters = 10m,
            PricePerLiter = 1.5m,
            Kilometars = 40900,
            CarId = 103
        }
    ];

    public IReadOnlyList<FuelExpense> GetAll() => _fuelExpenses;

    public FuelExpense? GetById(int id) => _fuelExpenses.FirstOrDefault(fuelExpense => fuelExpense.Id == id);
}
