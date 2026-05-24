using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Api.Dtos;

public sealed class InsuranceDto
{
    public int Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string InsuranceType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CarId { get; set; }
    public CarSummaryDto? Car { get; set; }
}

public sealed class InsuranceCreateDto
{
    [Required]
    public string Company { get; set; } = string.Empty;

    [Required]
    public string InsuranceType { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required]
    public int CarId { get; set; }
}

public sealed class InsuranceUpdateDto
{
    [Required]
    public string Company { get; set; } = string.Empty;

    [Required]
    public string InsuranceType { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required]
    public int CarId { get; set; }
}
