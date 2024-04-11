using CombatService.Data;
using CombatService.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace CombatService.Extensions;

public static class ExtensionConfigurations
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CombatDbContext>();
            dbContext.Database.Migrate();
        }
    }

    public static void SeedData(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<CombatDbContext>();

            context.Database.EnsureCreated();

            // Seed duels
            if (!context.Duels.Any())
            {
                var duels = new List<Duel>
                {
                    new Duel
                    {
                        ChallengerId = 1,
                        ChallengeeId = 2,
                        StartTime = DateTime.UtcNow,
                        Status = DuelStatus.Ongoing
                    },
                    new Duel
                    {
                        ChallengerId = 3,
                        ChallengeeId = 4,
                        StartTime = DateTime.UtcNow,
                        Status = DuelStatus.Ongoing
                    }
                };

                context.Duels.AddRange(duels);
                context.SaveChanges();
            }

            // Seed duel actions (optional)
            if (!context.DuelActions.Any())
            {
                var duelActions = new List<DuelAction>
                {
                    new DuelAction
                    {
                        DuelId = 1,
                        ActionType = DuelActionType.Attack,
                        CharacterId = 1,
                        Timestamp = DateTime.UtcNow
                    },
                    new DuelAction
                    {
                        DuelId = 1,
                        ActionType = DuelActionType.Heal,
                        CharacterId = 2,
                        Timestamp = DateTime.UtcNow.AddSeconds(1)
                    }
                };

                context.DuelActions.AddRange(duelActions);
                context.SaveChanges();
            }
        }
    }

    public static void AddRabbitMQ(this WebApplicationBuilder builder, Action<ConnectionFactory> configureConnectionFactory)
    {
        // Create and configure the connection factory
        var connectionFactory = new ConnectionFactory();
        configureConnectionFactory(connectionFactory);

        // Create the connection and channel
        var connection = connectionFactory.CreateConnection();
        var channel = connection.CreateModel();

        // Declare an exchange (you can also declare queues and bindings here if needed)
        channel.ExchangeDeclare(exchange: "combat_exchange", type: ExchangeType.Topic);

        // Register the channel as a singleton service
        builder.Services.AddSingleton(channel);
    }
}
