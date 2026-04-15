using CarExpanses.Enums;
using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class CarMockRepository
{
    private readonly List<Car> _cars =
    [
        new Car
        {
            Id = 101,
            UserId = 1,
            Brand = "Volkswagen",
            Model = "Golf 7",
            Year = 2018,
            EngineVolume = 1.6,
            CurrentMilage = 124500,
            PurchasePrice = 14200m,
            PurchaseDate = new DateTime(2020, 4, 18),
            FuelType = FuelType.Diesel,
            FuelExpenses = new List<FuelExpense>(),
            ServiceRecords = new List<ServiceRecord>(),
            Insurances = new List<Insurance>(),
            CarTires = new List<CarTire>(),
            Expenses = new List<Expense>()
        },
        new Car
        {
            Id = 102,
            UserId = 2,
            Brand = "Toyota",
            Model = "Corolla Hybrid",
            Year = 2021,
            EngineVolume = 1.8,
            CurrentMilage = 68400,
            PurchasePrice = 22600m,
            PurchaseDate = new DateTime(2022, 3, 12),
            FuelType = FuelType.Hybrid,
            FuelExpenses = new List<FuelExpense>(),
            ServiceRecords = new List<ServiceRecord>(),
            Insurances = new List<Insurance>(),
            CarTires = new List<CarTire>(),
            Expenses = new List<Expense>()
        },
        new Car
        {
            Id = 103,
            UserId = 3,
            Brand = "Tesla",
            Model = "Model 3",
            Year = 2022,
            EngineVolume = 0.0,
            CurrentMilage = 41200,
            PurchasePrice = 38900m,
            PurchaseDate = new DateTime(2023, 7, 1),
            FuelType = FuelType.Electric,
            FuelExpenses = new List<FuelExpense>(),
            ServiceRecords = new List<ServiceRecord>(),
            Insurances = new List<Insurance>(),
            CarTires = new List<CarTire>(),
            Expenses = new List<Expense>()
        }
    ];

    public IReadOnlyList<Car> GetAll() => _cars;

    public Car? GetById(int id) => _cars.FirstOrDefault(car => car.Id == id);
}
