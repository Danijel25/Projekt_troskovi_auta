using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Models;

public class InsuranceFormViewModel
{
    public int Id { get; set; }

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

    public IEnumerable<SelectListItem> CarOptions { get; set; } = [];
}
