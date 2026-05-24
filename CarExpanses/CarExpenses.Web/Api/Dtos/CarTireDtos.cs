using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Api.Dtos;

public sealed class CarTireDto
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int TireId { get; set; }
    public DateTime InstalledDate { get; set; }
    public CarSummaryDto? Car { get; set; }
    public TireSummaryDto? Tire { get; set; }
}

public sealed class CarTireCreateDto
{
    [Required]
    public int CarId { get; set; }

    [Required]
    public int TireId { get; set; }

    [DataType(DataType.Date)]
    public DateTime InstalledDate { get; set; }
}

public sealed class CarTireUpdateDto
{
    [Required]
    public int CarId { get; set; }

    [Required]
    public int TireId { get; set; }

    [DataType(DataType.Date)]
    public DateTime InstalledDate { get; set; }
}
