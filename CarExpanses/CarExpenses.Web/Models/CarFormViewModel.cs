using System.ComponentModel.DataAnnotations;
using CarExpenses.Model.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Models;

public class CarFormViewModel
{
    public int Id { get; set; }

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

    public IEnumerable<SelectListItem> UserOptions { get; set; } = [];
}
