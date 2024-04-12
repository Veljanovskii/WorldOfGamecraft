namespace AccountService.Models.DataTransferObjects;

public class RegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
}
