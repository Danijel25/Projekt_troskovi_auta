using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Models;

public class ExpenseFormViewModel
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public IEnumerable<SelectListItem> CategoryOptions { get; set; } = [];
}
