using CharacterService.Data;
using CharacterService.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace CharacterService.Extensions;

public static class ExtensionConfigurations
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CharacterDbContext>();
            dbContext.Database.Migrate();
        }
    }

    public static void SeedData(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<CharacterDbContext>();

            context.Database.EnsureCreated();

            // Seed classes
            if (!context.Classes.Any())
            {
                var classes = new List<Class>
                {
                    new() { Name = "Warrior", Description = "A brave fighter" },
                    new() { Name = "Mage", Description = "A powerful sorcerer" }
                };

                context.Classes.AddRange(classes);
                context.SaveChanges();
            }

            // Seed items
            if (!context.Items.Any())
            {
                var items = new List<Item>
                {
                    new() { Name = "Sword", Description = "A sharp blade", BonusStrength = 5 },
                    new() { Name = "Staff", Description = "A magical wand", BonusIntelligence = 5 }
                };

                context.Items.AddRange(items);
                context.SaveChanges();
            }

            // Seed characters
            if (!context.Characters.Any())
            {
                var characters = new List<Character>
                {
                    new() 
                    { 
                        Name = "John the Warrior", 
                        ClassId = 1, Health = 100, 
                        Mana = 50, 
                        BaseStrength = 10, 
                        BaseAgility = 5, 
                        BaseIntelligence = 3, 
                        BaseFaith = 2,
                        CreatedById = 1
                    },
                    new() 
                    { 
                        Name = "Jane the Mage", 
                        ClassId = 2, Health = 80, 
                        Mana = 100, 
                        BaseStrength = 3, 
                        BaseAgility = 4, 
                        BaseIntelligence = 10, 
                        BaseFaith = 5,
                        CreatedById = 2
                    }
                };

                context.Characters.AddRange(characters);
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
        channel.ExchangeDeclare(exchange: "character_exchange", type: ExchangeType.Topic);

        // Register the channel as a singleton service
        builder.Services.AddSingleton(channel);
    }
}
