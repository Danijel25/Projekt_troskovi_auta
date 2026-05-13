using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Models;

public class FuelExpenseFormViewModel
{
    public int Id { get; set; }

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

    public IEnumerable<SelectListItem> CarOptions { get; set; } = [];
}
