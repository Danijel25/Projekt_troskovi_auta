using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Model.Models;
public class User
{
    [Key]
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public virtual ICollection<Car>? Cars { get; set; } = new List<Car>();
}
