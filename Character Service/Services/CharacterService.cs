using CharacterService.Data;
using CharacterService.Models;
using CharacterService.Models.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace CharacterService.Services;

public class CharacterService(CharacterDbContext context,
    IDistributedCache cache,
    JsonSerializerOptions jsonOptions,
    IModel channel) : ICharacterService
{
    private readonly CharacterDbContext _context = context;
    private readonly IDistributedCache _cache = cache;
    private readonly JsonSerializerOptions _jsonOptions = jsonOptions;
    private readonly IModel _channel = channel;

    public async Task<IEnumerable<CharacterDto>> GetCharactersAsync()
    {
        return await _context.Characters
            .Select(c => new CharacterDto
            {
                Id = c.Id,
                Name = c.Name,
                Health = c.Health,
                Mana = c.Mana
            })
            .ToListAsync();
    }

    public async Task<CharacterDetailDto> GetCharacterByIdAsync(int id)
    {
        var cacheKey = $"Character_{id}";
        var cachedCharacter = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedCharacter))
        {
            return JsonSerializer.Deserialize<CharacterDetailDto>(cachedCharacter, _jsonOptions);
        }

        var character = await _context.Characters
            .Where(c => c.Id == id)
            .Select(c => new CharacterDetailDto
            {
                Id = c.Id,
                Name = c.Name,
                Health = c.Health,
                Mana = c.Mana,
                BaseStrength = c.BaseStrength,
                BaseAgility = c.BaseAgility,
                BaseIntelligence = c.BaseIntelligence,
                BaseFaith = c.BaseFaith,
                TotalStrength = c.BaseStrength + c.Items.Sum(i => i.BonusStrength),
                TotalAgility = c.BaseAgility + c.Items.Sum(i => i.BonusAgility),
                TotalIntelligence = c.BaseIntelligence + c.Items.Sum(i => i.BonusIntelligence),
                TotalFaith = c.BaseFaith + c.Items.Sum(i => i.BonusFaith),
                Items = c.Items.Select(i => new ItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    BonusStrength = i.BonusStrength,
                    BonusAgility = i.BonusAgility,
                    BonusIntelligence = i.BonusIntelligence,
                    BonusFaith = i.BonusFaith
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (character != null)
        {
            var serializedCharacter = JsonSerializer.Serialize(character, _jsonOptions);
            await _cache.SetStringAsync(cacheKey, serializedCharacter, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // Cache expiration
            });
        }

        return character;
    }

    public async Task<CharacterDetailDto> CreateCharacterAsync(CharacterCreateDto characterDto, int createdById)
    {
        var character = new Character
        {
            Name = characterDto.Name,
            ClassId = characterDto.ClassId,
            Health = characterDto.Health,
            Mana = characterDto.Mana,
            BaseStrength = characterDto.BaseStrength,
            BaseAgility = characterDto.BaseAgility,
            BaseIntelligence = characterDto.BaseIntelligence,
            BaseFaith = characterDto.BaseFaith,
            CreatedById = createdById
        };

        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        // Publish character creation message to RabbitMQ
        PublishCharacterCreationMessage(character);

        return new CharacterDetailDto
        {
            Id = character.Id,
            Name = character.Name,
            // Other properties...
        };
    }

    private void PublishCharacterCreationMessage(Character character)
    {
        var message = new
        {
            CharacterId = character.Id,
            UserId = character.CreatedById
        };
        var messageBody = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageBody);

        // Assuming you have a RabbitMQ channel set up and available
        _channel.BasicPublish(exchange: "character_exchange",
                             routingKey: "character.created",
                             basicProperties: null,
                             body: body);
    }

    public async Task<IEnumerable<ItemDto>> GetItemsAsync()
    {
        return await _context.Items
            .Select(i => new ItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                BonusStrength = i.BonusStrength,
                BonusAgility = i.BonusAgility,
                BonusIntelligence = i.BonusIntelligence,
                BonusFaith = i.BonusFaith
            })
            .ToListAsync();
    }

    public async Task<ItemDto> CreateItemAsync(ItemCreateDto itemDto)
    {
        var item = new Item
        {
            Name = itemDto.Name,
            Description = itemDto.Description,
            BonusStrength = itemDto.BonusStrength,
            BonusAgility = itemDto.BonusAgility,
            BonusIntelligence = itemDto.BonusIntelligence,
            BonusFaith = itemDto.BonusFaith
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            BonusStrength = item.BonusStrength,
            BonusAgility = item.BonusAgility,
            BonusIntelligence = item.BonusIntelligence,
            BonusFaith = item.BonusFaith
        };
    }

    public async Task<ItemDto> GetItemByIdAsync(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return null;
        }

        var suffix = GetItemSuffix(item);
        var itemName = $"{item.Name} {suffix}";

        return new ItemDto
        {
            Id = item.Id,
            Name = itemName,
            Description = item.Description,
            BonusStrength = item.BonusStrength,
            BonusAgility = item.BonusAgility,
            BonusIntelligence = item.BonusIntelligence,
            BonusFaith = item.BonusFaith
        };
    }

    private string GetItemSuffix(Item item)
    {
        var maxStat = new[] { item.BonusStrength, item.BonusAgility, item.BonusIntelligence, item.BonusFaith }.Max();
        if (maxStat == item.BonusStrength) return "Of The Bear";
        if (maxStat == item.BonusAgility) return "Of The Cobra";
        if (maxStat == item.BonusIntelligence) return "Of The Owl";
        if (maxStat == item.BonusFaith) return "Of The Unicorn";
        return ""; // In case of a tie or no bonuses
    }

    public async Task<bool> GrantItemAsync(ItemGrantDto grantDto)
    {
        var character = await _context.Characters
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == grantDto.CharacterId);

        if (character == null)
        {
            return false;
        }

        var item = await _context.Items.FindAsync(grantDto.ItemId);
        if (item == null)
        {
            return false;
        }

        character.Items.Add(item);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> GiftItemAsync(ItemGiftDto giftDto)
    {
        var fromCharacter = await _context.Characters
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == giftDto.FromCharacterId);

        if (fromCharacter == null)
        {
            return false;
        }

        var toCharacter = await _context.Characters
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == giftDto.ToCharacterId);

        if (toCharacter == null)
        {
            return false;
        }

        var item = fromCharacter.Items.FirstOrDefault(i => i.Id == giftDto.ItemId);
        if (item == null)
        {
            return false;
        }

        fromCharacter.Items.Remove(item);
        toCharacter.Items.Add(item);
        await _context.SaveChangesAsync();

        return true;
    }
}
