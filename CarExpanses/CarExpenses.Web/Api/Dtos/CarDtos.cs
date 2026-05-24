using System.ComponentModel.DataAnnotations;
using CarExpenses.Model.Enums;

namespace CarExpenses.Web.Api.Dtos;

public sealed class CarListItemDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserSummaryDto? User { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public FuelType FuelType { get; set; }
    public int CurrentMilage { get; set; }
}

public sealed class CarDetailDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserSummaryDto? User { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public double EngineVolume { get; set; }
    public int CurrentMilage { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public FuelType FuelType { get; set; }
    public IReadOnlyList<FuelExpenseForCarDto> FuelExpenses { get; set; } = [];
    public IReadOnlyList<ServiceRecordForCarDto> ServiceRecords { get; set; } = [];
    public IReadOnlyList<InsuranceForCarDto> Insurances { get; set; } = [];
    public IReadOnlyList<CarTireForCarDto> CarTires { get; set; } = [];
    public IReadOnlyList<ExpenseForCarDto> Expenses { get; set; } = [];
}

public sealed class FuelExpenseForCarDto
{
    public int Id { get; set; }
    public DateTime FuelExpenseDate { get; set; }
    public decimal Liters { get; set; }
    public decimal PricePerLiter { get; set; }
    public decimal TotalCost { get; set; }
    public int Kilometars { get; set; }
}

public sealed class ServiceRecordForCarDto
{
    public int Id { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public DateTime ServiceDate { get; set; }
    public int Mileage { get; set; }
}

public sealed class InsuranceForCarDto
{
    public int Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string InsuranceType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public sealed class CarTireForCarDto
{
    public int Id { get; set; }
    public int TireId { get; set; }
    public TireSummaryDto? Tire { get; set; }
    public DateTime InstalledDate { get; set; }
}

public sealed class ExpenseForCarDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public ExpenseCategoryDto? Category { get; set; }
}

public sealed class CarCreateDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(64)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string Model { get; set; } = string.Empty;

    [Range(1950, 2100)]
    public int Year { get; set; }

    [Range(0, double.MaxValue)]
    public double EngineVolume { get; set; }

    [Range(0, int.MaxValue)]
    public int CurrentMilage { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PurchasePrice { get; set; }

    [DataType(DataType.Date)]
    public DateTime PurchaseDate { get; set; }

    public FuelType FuelType { get; set; }
}

public sealed class CarUpdateDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(64)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string Model { get; set; } = string.Empty;

    [Range(1950, 2100)]
    public int Year { get; set; }

    [Range(0, double.MaxValue)]
    public double EngineVolume { get; set; }

    [Range(0, int.MaxValue)]
    public int CurrentMilage { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PurchasePrice { get; set; }

    [DataType(DataType.Date)]
    public DateTime PurchaseDate { get; set; }

    public FuelType FuelType { get; set; }
}
