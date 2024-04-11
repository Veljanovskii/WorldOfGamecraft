using AccountService.Data;
using AccountService.Models.DataTransferObjects;
using AccountService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountService.Services;

public class AccountService(AccountDbContext context) : IAccountService
{
    private readonly AccountDbContext _context = context;

    public async Task<ServiceResult> RegisterUserAsync(RegisterDto registerDto)
    {
        if (await _context.Accounts.AnyAsync(a => a.Username == registerDto.Username))
        {
            return new ServiceResult { Success = false, Message = "Username already exists." };
        }

        var user = new Account
        {
            Username = registerDto.Username,
            PasswordHash = HashPassword(registerDto.Password),
            RoleId = registerDto.RoleId
        };

        _context.Accounts.Add(user);
        await _context.SaveChangesAsync();

        return new ServiceResult { Success = true, Message = "User registered successfully." };
    }

    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Username == loginDto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return null; // Authentication failed
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("YourWorldOfGamecraftSuperSecretKeyShouldBeHidden");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
