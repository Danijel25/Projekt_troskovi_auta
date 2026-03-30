namespace CarExpanses.Models;
public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public List<Car>? Cars { get; set; }
}