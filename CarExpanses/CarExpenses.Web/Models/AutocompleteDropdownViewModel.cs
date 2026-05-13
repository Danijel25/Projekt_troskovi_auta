namespace CarExpenses.Web.Models;

public class AutocompleteDropdownViewModel
{
    public string InputId { get; set; } = string.Empty;
    public string HiddenInputId { get; set; } = string.Empty;
    public string HiddenInputName { get; set; } = string.Empty;
    public string LookupUrl { get; set; } = string.Empty;
    public string Placeholder { get; set; } = "Select";
    public string SelectedLabel { get; set; } = string.Empty;
    public string SelectedValue { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public int MinChars { get; set; } = 1;
    public int Limit { get; set; } = 25;
}
