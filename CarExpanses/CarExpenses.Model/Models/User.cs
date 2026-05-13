using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Model.Models;
using CarExpenses.Model;
public class User : ISoftDeleate
{
    [Key]
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public virtual ICollection<Car>? Cars { get; set; } = new List<Car>();
    public DateTime? DeleatedAt { get; set; }
}
