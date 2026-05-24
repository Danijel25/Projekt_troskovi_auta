using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Api.Dtos;

public sealed class TireDetailDto
{
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public IReadOnlyList<CarTireForTireDto> CarTires { get; set; } = [];
}

public sealed class CarTireForTireDto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public CarSummaryDto? Car { get; set; }
    public DateTime InstalledDate { get; set; }
}

public sealed class TireCreateDto
{
    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    [Required]
    public string Season { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}

public sealed class TireUpdateDto
{
    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    [Required]
    public string Season { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
