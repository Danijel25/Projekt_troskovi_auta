using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Models;

public class TireFormViewModel
{
    public int Id { get; set; }

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    [Required]
    public string Season { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
