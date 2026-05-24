using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;

namespace CarExpenses.Web.Api.Mapping;

internal static class DtoMapping
{
    public static UserSummaryDto ToSummaryDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email
    };

    public static UserDetailDto ToDetailDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        Cars = user.Cars?.Select(ToSummaryDto).ToList() ?? []
    };

    public static CarSummaryDto ToSummaryDto(Car car) => new()
    {
        Id = car.Id,
        UserId = car.UserId,
        Brand = car.Brand,
        Model = car.Model,
        Year = car.Year,
        FuelType = car.FuelType,
        CurrentMilage = car.CurrentMilage
    };

    public static CarListItemDto ToListItemDto(Car car) => new()
    {
        Id = car.Id,
        UserId = car.UserId,
        User = car.User is null ? null : ToSummaryDto(car.User),
        Brand = car.Brand,
        Model = car.Model,
        Year = car.Year,
        FuelType = car.FuelType,
        CurrentMilage = car.CurrentMilage
    };

    public static CarDetailDto ToDetailDto(Car car) => new()
    {
        Id = car.Id,
        UserId = car.UserId,
        User = car.User is null ? null : ToSummaryDto(car.User),
        Brand = car.Brand,
        Model = car.Model,
        Year = car.Year,
        EngineVolume = car.EngineVolume,
        CurrentMilage = car.CurrentMilage,
        PurchasePrice = car.PurchasePrice,
        PurchaseDate = car.PurchaseDate,
        FuelType = car.FuelType,
        FuelExpenses = car.FuelExpenses?.Select(ToForCarDto).ToList() ?? [],
        ServiceRecords = car.ServiceRecords?.Select(ToForCarDto).ToList() ?? [],
        Insurances = car.Insurances?.Select(ToForCarDto).ToList() ?? [],
        CarTires = car.CarTires?.Select(ToForCarDto).ToList() ?? [],
        Expenses = car.Expenses?.Select(ToForCarDto).ToList() ?? []
    };

    public static TireSummaryDto ToSummaryDto(Tire tire) => new()
    {
        Id = tire.Id,
        Brand = tire.Brand,
        Model = tire.Model,
        Season = tire.Season,
        Price = tire.Price
    };

    public static TireDetailDto ToDetailDto(Tire tire) => new()
    {
        Id = tire.Id,
        Brand = tire.Brand,
        Model = tire.Model,
        Season = tire.Season,
        Price = tire.Price,
        CarTires = tire.CarTires?.Select(ToForTireDto).ToList() ?? []
    };

    public static CarTireDto ToDto(CarTire carTire) => new()
    {
        Id = carTire.Id,
        CarId = carTire.CarId,
        TireId = carTire.TireId,
        InstalledDate = carTire.InstalledDate,
        Car = carTire.Car is null ? null : ToSummaryDto(carTire.Car),
        Tire = carTire.Tire is null ? null : ToSummaryDto(carTire.Tire)
    };

    public static CarTireForCarDto ToForCarDto(CarTire carTire) => new()
    {
        Id = carTire.Id,
        TireId = carTire.TireId,
        Tire = carTire.Tire is null ? null : ToSummaryDto(carTire.Tire),
        InstalledDate = carTire.InstalledDate
    };

    public static CarTireForTireDto ToForTireDto(CarTire carTire) => new()
    {
        Id = carTire.Id,
        CarId = carTire.CarId,
        Car = carTire.Car is null ? null : ToSummaryDto(carTire.Car),
        InstalledDate = carTire.InstalledDate
    };

    public static FuelExpenseDto ToDto(FuelExpense fuelExpense) => new()
    {
        Id = fuelExpense.Id,
        FuelExpenseDate = fuelExpense.FuelExpenseDate,
        Liters = fuelExpense.Liters,
        PricePerLiter = fuelExpense.PricePerLiter,
        TotalCost = fuelExpense.TotalCost,
        Kilometars = fuelExpense.Kilometars,
        CarId = fuelExpense.CarId,
        Car = fuelExpense.Car is null ? null : ToSummaryDto(fuelExpense.Car)
    };

    public static FuelExpenseForCarDto ToForCarDto(FuelExpense fuelExpense) => new()
    {
        Id = fuelExpense.Id,
        FuelExpenseDate = fuelExpense.FuelExpenseDate,
        Liters = fuelExpense.Liters,
        PricePerLiter = fuelExpense.PricePerLiter,
        TotalCost = fuelExpense.TotalCost,
        Kilometars = fuelExpense.Kilometars
    };

    public static ServiceRecordDto ToDto(ServiceRecord serviceRecord) => new()
    {
        Id = serviceRecord.Id,
        ServiceType = serviceRecord.ServiceType,
        Description = serviceRecord.Description,
        Cost = serviceRecord.Cost,
        ServiceDate = serviceRecord.ServiceDate,
        Mileage = serviceRecord.Mileage,
        CarId = serviceRecord.CarId,
        Car = serviceRecord.Car is null ? null : ToSummaryDto(serviceRecord.Car)
    };

    public static ServiceRecordForCarDto ToForCarDto(ServiceRecord serviceRecord) => new()
    {
        Id = serviceRecord.Id,
        ServiceType = serviceRecord.ServiceType,
        Description = serviceRecord.Description,
        Cost = serviceRecord.Cost,
        ServiceDate = serviceRecord.ServiceDate,
        Mileage = serviceRecord.Mileage
    };

    public static InsuranceDto ToDto(Insurance insurance) => new()
    {
        Id = insurance.Id,
        Company = insurance.Company,
        InsuranceType = insurance.InsuranceType,
        Price = insurance.Price,
        StartDate = insurance.StartDate,
        EndDate = insurance.EndDate,
        CarId = insurance.CarId,
        Car = insurance.Car is null ? null : ToSummaryDto(insurance.Car)
    };

    public static InsuranceForCarDto ToForCarDto(Insurance insurance) => new()
    {
        Id = insurance.Id,
        Company = insurance.Company,
        InsuranceType = insurance.InsuranceType,
        Price = insurance.Price,
        StartDate = insurance.StartDate,
        EndDate = insurance.EndDate
    };

    public static ExpenseCategoryDto ToDto(ExpenseCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name
    };

    public static ExpenseCategoryDetailDto ToDetailDto(ExpenseCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Expenses = category.Expenses?.Select(ToSummaryDto).ToList() ?? []
    };

    public static ExpenseSummaryDto ToSummaryDto(Expense expense) => new()
    {
        Id = expense.Id,
        Description = expense.Description,
        Amount = expense.Amount,
        Date = expense.Date
    };

    public static ExpenseForCarDto ToForCarDto(Expense expense) => new()
    {
        Id = expense.Id,
        Description = expense.Description,
        Amount = expense.Amount,
        Date = expense.Date,
        CategoryId = expense.CategoryId,
        Category = expense.Category is null ? null : ToDto(expense.Category)
    };

    public static CarFileDto ToDto(CarFile carFile, string fileUrl) => new()
    {
        Id = carFile.Id,
        FileName = carFile.OriginalFileName,
        ContentType = carFile.ContentType,
        FileSize = carFile.FileSize,
        UploadedAt = carFile.UploadedAt,
        Url = fileUrl
    };
}
