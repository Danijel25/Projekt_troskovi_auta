using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Api.Dtos;

public sealed class FuelExpenseDto
{
    public int Id { get; set; }
    public DateTime FuelExpenseDate { get; set; }
    public decimal Liters { get; set; }
    public decimal PricePerLiter { get; set; }
    public decimal TotalCost { get; set; }
    public int Kilometars { get; set; }
    public int CarId { get; set; }
    public CarSummaryDto? Car { get; set; }
}

public sealed class FuelExpenseCreateDto
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime FuelExpenseDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Liters { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PricePerLiter { get; set; }

    [Range(0, int.MaxValue)]
    public int Kilometars { get; set; }

    [Required]
    public int CarId { get; set; }
}

public sealed class FuelExpenseUpdateDto
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime FuelExpenseDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Liters { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PricePerLiter { get; set; }

    [Range(0, int.MaxValue)]
    public int Kilometars { get; set; }

    [Required]
    public int CarId { get; set; }
}
