public class User
{
    //properties: id, username, email, password
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<Car> Cars { get; set; }
}