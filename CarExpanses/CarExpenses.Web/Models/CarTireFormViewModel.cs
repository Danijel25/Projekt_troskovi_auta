using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Models;

public class CarTireFormViewModel
{
    public int Id { get; set; }

    [Required]
    public int CarId { get; set; }

    [Required]
    public int TireId { get; set; }

    [DataType(DataType.Date)]
    public DateTime InstalledDate { get; set; }

    public IEnumerable<SelectListItem> CarOptions { get; set; } = [];
    public IEnumerable<SelectListItem> TireOptions { get; set; } = [];
}
