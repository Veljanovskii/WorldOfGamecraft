namespace AccountService.Models;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
}
