using CarExpanses.Enums;
using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class CarMockRepository
{
    private readonly List<Car> _cars;

    public CarMockRepository()
    {
        var fuelExpenseRepository = new FuelExpenseMockRepository();
        var serviceRecordRepository = new ServiceRecordMockRepository();
        var insuranceRepository = new InsuranceMockRepository();
        var carTireRepository = new CarTireMockRepository();
        var tireRepository = new TireMockRepository();
        var expenseRepository = new ExpenseMockRepository();

        var car1 = new Car
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
        };

        var car2 = new Car
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
        };

        var car3 = new Car
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
        };

        var tiresById = tireRepository.GetAll().ToDictionary(tire => tire.Id);

        car1.FuelExpenses = fuelExpenseRepository.GetAll().Where(expense => expense.CarId == car1.Id).ToList();
        car1.ServiceRecords = serviceRecordRepository.GetAll().Where(record => record.CarId == car1.Id).ToList();
        car1.Insurances = insuranceRepository.GetAll().Where(insurance => insurance.CarId == car1.Id).ToList();
        car1.CarTires = carTireRepository.GetAll().Where(carTire => carTire.CarId == car1.Id).ToList();
        car1.Expenses = expenseRepository.GetAll().Take(2).ToList();

        car2.FuelExpenses = fuelExpenseRepository.GetAll().Where(expense => expense.CarId == car2.Id).ToList();
        car2.ServiceRecords = serviceRecordRepository.GetAll().Where(record => record.CarId == car2.Id).ToList();
        car2.Insurances = insuranceRepository.GetAll().Where(insurance => insurance.CarId == car2.Id).ToList();
        car2.CarTires = carTireRepository.GetAll().Where(carTire => carTire.CarId == car2.Id).ToList();
        car2.Expenses = expenseRepository.GetAll().Skip(2).Take(2).ToList();

        car3.FuelExpenses = fuelExpenseRepository.GetAll().Where(expense => expense.CarId == car3.Id).ToList();
        car3.ServiceRecords = serviceRecordRepository.GetAll().Where(record => record.CarId == car3.Id).ToList();
        car3.Insurances = insuranceRepository.GetAll().Where(insurance => insurance.CarId == car3.Id).ToList();
        car3.CarTires = carTireRepository.GetAll().Where(carTire => carTire.CarId == car3.Id).ToList();
        car3.Expenses = expenseRepository.GetAll().Skip(4).Take(2).ToList();

        _cars = new List<Car> { car1, car2, car3 };
    }

    public IReadOnlyList<Car> GetAll() => _cars;

    public Car? GetById(int id) => _cars.FirstOrDefault(car => car.Id == id);
}
