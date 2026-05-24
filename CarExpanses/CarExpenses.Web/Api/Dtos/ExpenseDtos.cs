using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Api.Dtos;

public sealed class ExpenseListItemDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public ExpenseCategoryDto? Category { get; set; }
    public int? CarId { get; set; }
}

public sealed class ExpenseDetailDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public ExpenseCategoryDto? Category { get; set; }
    public int? CarId { get; set; }
    public CarSummaryDto? Car { get; set; }
}

public sealed class ExpenseCreateDto
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public int? CarId { get; set; }
}

public sealed class ExpenseUpdateDto
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public int? CarId { get; set; }
}
