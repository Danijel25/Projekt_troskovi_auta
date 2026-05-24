using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Web.Api.Dtos;

public sealed class ExpenseCategoryDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<ExpenseSummaryDto> Expenses { get; set; } = [];
}

public sealed class ExpenseCategoryCreateDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; } = string.Empty;
}

public sealed class ExpenseCategoryUpdateDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; } = string.Empty;
}
