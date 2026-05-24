using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Api.Dtos;

public sealed class ServiceRecordDto
{
    public int Id { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public DateTime ServiceDate { get; set; }
    public int Mileage { get; set; }
    public int CarId { get; set; }
    public CarSummaryDto? Car { get; set; }
}

public sealed class ServiceRecordCreateDto
{
    [Required]
    public string ServiceType { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Cost { get; set; }

    [DataType(DataType.Date)]
    public DateTime ServiceDate { get; set; }

    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }

    [Required]
    public int CarId { get; set; }
}

public sealed class ServiceRecordUpdateDto
{
    [Required]
    public string ServiceType { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Cost { get; set; }

    [DataType(DataType.Date)]
    public DateTime ServiceDate { get; set; }

    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }

    [Required]
    public int CarId { get; set; }
}
