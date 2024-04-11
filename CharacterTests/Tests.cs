using CharacterService.Data;
using CharacterService.Models;
using CharacterService.Models.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text.Json;

namespace CharacterTests;

[TestFixture]
public class Tests
{
    [Test]
    public async Task GetCharactersAsync_CharactersExist_ReturnsListOfCharacters()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CharacterDbContext>()
            .UseInMemoryDatabase(databaseName: "GetCharactersTestDb")
            .Options;

        using (var context = new CharacterDbContext(options))
        {
            context.Characters.Add(new Character { Id = 1, Name = "Test Character 1" });
            context.Characters.Add(new Character { Id = 2, Name = "Test Character 2" });
            context.SaveChanges();
        }

        using (var context = new CharacterDbContext(options))
        {
            var service = new CharacterService.Services.CharacterService(context,
                new Mock<IDistributedCache>().Object, 
                new JsonSerializerOptions());

            // Act
            var characters = await service.GetCharactersAsync();

            // Assert
            Assert.That(characters, Is.Not.Empty);
            Assert.That(characters, Has.Exactly(2).Items);
            Assert.That(characters, Has.Some.Property("Name").EqualTo("Test Character 1"));
            Assert.That(characters, Has.Some.Property("Name").EqualTo("Test Character 2"));
        }
    }

    [Test]
    public async Task GetCharacterByIdAsync_CharacterExists_ReturnsCharacter()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CharacterDbContext>()
            .UseInMemoryDatabase(databaseName: "CharacterTestDb")
            .Options;

        var mockCache = new Mock<IDistributedCache>();

        var character = new Character { Id = 1, Name = "Test Character" };

        using (var context = new CharacterDbContext(options))
        {
            context.Characters.Add(character);
            context.SaveChanges();
        }

        using (var context = new CharacterDbContext(options))
        {
            var characterService = new CharacterService.Services.CharacterService(context, 
                mockCache.Object, 
                new JsonSerializerOptions());

            // Act
            var result = await characterService.GetCharacterByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Test Character"));
        }
    }

    [Test]
    public async Task CreateCharacterAsync_ValidCharacter_ReturnsCreatedCharacter()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CharacterDbContext>()
            .UseInMemoryDatabase(databaseName: "CreateCharacterTestDb")
            .Options;

        var characterDto = new CharacterCreateDto
        {
            Name = "New Character",
            ClassId = 1,
            Health = 100,
            Mana = 50,
            BaseStrength = 10,
            BaseAgility = 5,
            BaseIntelligence = 3,
            BaseFaith = 2
        };

        int createdById = 1; // Hard-coded user ID for testing

        using (var context = new CharacterDbContext(options))
        {
            var service = new CharacterService.Services.CharacterService(context, 
                new Mock<IDistributedCache>().Object, 
                new JsonSerializerOptions());

            // Act
            var result = await service.CreateCharacterAsync(characterDto, createdById);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("New Character"));
            Assert.That(context.Characters.FirstOrDefault(c => c.Id == result.Id)?.CreatedById, Is.EqualTo(createdById));
        }
    }

    [Test]
    public async Task GetItemsAsync_ItemsExist_ReturnsListOfItems()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CharacterDbContext>()
            .UseInMemoryDatabase(databaseName: "GetItemsTestDb")
            .Options;

        using (var context = new CharacterDbContext(options))
        {
            context.Items.Add(new Item { Name = "Test Item", Description = "A test item", BonusStrength = 5 });
            context.SaveChanges();
        }

        using (var context = new CharacterDbContext(options))
        {
            var service = new CharacterService.Services.CharacterService(context, 
                new Mock<IDistributedCache>().Object, 
                new JsonSerializerOptions());

            // Act
            var items = await service.GetItemsAsync();

            // Assert
            Assert.That(items, Is.Not.Empty);
            Assert.That(items.First().Name, Is.EqualTo("Test Item"));
        }
    }

    [Test]
    public async Task CreateItemAsync_ValidItem_ReturnsCreatedItem()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CharacterDbContext>()
            .UseInMemoryDatabase(databaseName: "CreateItemTestDb")
            .Options;

        var itemDto = new ItemCreateDto
        {
            Name = "New Item",
            Description = "A new test item",
            BonusStrength = 5
        };

        using (var context = new CharacterDbContext(options))
        {
            var service = new CharacterService.Services.CharacterService(context, 
                new Mock<IDistributedCache>().Object,
                new JsonSerializerOptions());

            // Act
            var result = await service.CreateItemAsync(itemDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("New Item"));
            Assert.That(result.Description, Is.EqualTo("A new test item"));
            Assert.That(result.BonusStrength, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task GrantItemAsync_ValidCharacterAndItem_ItemGrantedToCharacter()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CharacterDbContext>()
            .UseInMemoryDatabase(databaseName: "GrantItemTestDb")
            .Options;

        var character = new Character { Id = 1, Name = "Test Character" };
        var item = new Item { Id = 1, Name = "Test Item", Description = "Test Item Description" };

        using (var context = new CharacterDbContext(options))
        {
            context.Characters.Add(character);
            context.Items.Add(item);
            context.SaveChanges();
        }

        using (var context = new CharacterDbContext(options))
        {
            var service = new CharacterService.Services.CharacterService(context, 
                new Mock<IDistributedCache>().Object, 
                new JsonSerializerOptions());

            // Act
            var result = await service.GrantItemAsync(new ItemGrantDto { CharacterId = 1, ItemId = 1 });

            // Assert
            Assert.That(result, Is.True);
            Assert.That(context.Characters.Include(c => c.Items).First().Items, Has.Some.Property("Id").EqualTo(1));
        }
    }

    [Test]
    public async Task GiftItemAsync_ValidFromAndToCharactersAndItem_ItemMovedBetweenCharacters()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CharacterDbContext>()
            .UseInMemoryDatabase(databaseName: "GiftItemTestDb")
            .Options;

        var fromCharacter = new Character { Id = 1, Name = "From Character", Items = new List<Item>() };
        var toCharacter = new Character { Id = 2, Name = "To Character", Items = new List<Item>() };
        var item = new Item { Id = 1, Name = "Test Item", Description = "Test Item Description" };

        using (var context = new CharacterDbContext(options))
        {
            fromCharacter.Items.Add(item);
            context.Characters.AddRange(fromCharacter, toCharacter);
            context.SaveChanges();
        }

        using (var context = new CharacterDbContext(options))
        {
            var service = new CharacterService.Services.CharacterService(context, 
                new Mock<IDistributedCache>().Object,
                new JsonSerializerOptions());

            // Act
            var result = await service.GiftItemAsync(new ItemGiftDto { FromCharacterId = 1, ToCharacterId = 2, ItemId = 1 });

            // Assert
            Assert.That(result, Is.True);
            var updatedFromCharacter = context.Characters.Include(c => c.Items).First(c => c.Id == 1);
            var updatedToCharacter = context.Characters.Include(c => c.Items).First(c => c.Id == 2);
            Assert.That(updatedFromCharacter.Items, Is.Empty);
            Assert.That(updatedToCharacter.Items, Has.Some.Property("Id").EqualTo(1));
        }
    }
}