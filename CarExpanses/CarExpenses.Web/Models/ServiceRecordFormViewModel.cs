using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Models;

public class ServiceRecordFormViewModel
{
    public int Id { get; set; }

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

    public IEnumerable<SelectListItem> CarOptions { get; set; } = [];
}
