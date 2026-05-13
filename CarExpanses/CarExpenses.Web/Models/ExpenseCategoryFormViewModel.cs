using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Models;

public class ExpenseCategoryFormViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}
