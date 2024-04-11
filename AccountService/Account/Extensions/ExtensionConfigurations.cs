using AccountService.Data;
using AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Extensions;

public static class ExtensionConfigurations
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
            dbContext.Database.Migrate();
        }
    }

    public static void SeedData(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AccountDbContext>();

            context.Database.EnsureCreated();

            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new() { Name = "GameMaster" },
                    new() { Name = "User" }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges();

                if (!context.Accounts.Any())
                {
                    var accounts = new List<Account>
                    {
                        new() {
                            Username = "John Doe",
                            PasswordHash = "$2a$10$MsF4Ce/myDIG55Qy87Ucmu4ACNjXTa1Sk7Z/wd0pRtfRsst3B1Zyy",
                            RoleId = roles[0].Id
                        },
                        new Account {
                            Username = "Jane Doe",
                            PasswordHash = "$2a$10$vhXLXbIYKwyDTTA7DlpBAeDTkAnHkpUZwSyVZiF9G/BW5lihF5CLm",
                            RoleId = roles[1].Id
                        }
                    };

                    context.Accounts.AddRange(accounts);
                    context.SaveChanges();
                }
            }
        }
    }
}
