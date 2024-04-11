using CharacterService.Models;
using CharacterService.Models.DataTransferObjects;
using CharacterService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CharacterService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CharacterController(ICharacterService characterService,
    ILogger<CharacterController> logger) : ControllerBase
{
    private readonly ICharacterService _characterService = characterService;
    private readonly ILogger<CharacterController> _logger = logger;

    [HttpGet("character")]
    [Authorize(Roles = "GameMaster")]
    public async Task<IActionResult> GetCharacters()
    {
        _logger.LogInformation("Getting all characters");
        var characters = await _characterService.GetCharactersAsync();

        if (!characters.Any())
        {
            _logger.LogWarning("No characters found");
            return NoContent();
        }

        return Ok(characters);
    }

    [HttpGet("character/{id}")]
    public async Task<IActionResult> GetCharacterById(int id)
    {
        _logger.LogInformation($"Getting character with ID {id}");
        var character = await _characterService.GetCharacterByIdAsync(id);

        if (character == null)
        {
            _logger.LogWarning($"Character with ID {id} not found");
            return NotFound();
        }

        // Retrieve the current user's ID from the claims
        var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        // Check if the current user is the owner of the character or a GameMaster
        if (currentUserId != character.CreatedById.ToString() && !User.IsInRole("GameMaster"))
        {
            _logger.LogWarning($"User {currentUserId} is not authorized to access character {id}");
            return Forbid();
        }

        return Ok(character);
    }

    [HttpPost("character")]
    public async Task<IActionResult> CreateCharacter([FromBody] CharacterCreateDto characterDto)
    {
        _logger.LogInformation("Creating new character");
        // Extract user ID from the JWT token
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //Normal flow:
        /*if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID not found in token");
            return Unauthorized("User ID not found in token.");
        }
        var character = await _characterService.CreateCharacterAsync(characterDto, int.Parse(userId));*/

        //Alternative:
        var character = await _characterService.CreateCharacterAsync(characterDto, 1);

        if (character == null)
        {
            _logger.LogError("Failed to create character");
            return BadRequest("Unable to create character.");
        }
        return CreatedAtAction(nameof(GetCharacterById), new { id = character.Id }, character);
    }

    [HttpGet("items")]
    [Authorize(Roles = "GameMaster")]
    public async Task<IActionResult> GetItems()
    {
        _logger.LogInformation("Getting all items");
        var items = await _characterService.GetItemsAsync();

        if (!items.Any())
        {
            _logger.LogWarning("No items found");
            return NoContent();
        }

        return Ok(items);
    }

    [HttpPost("items")]
    [Authorize(Roles = "GameMaster")]
    public async Task<IActionResult> CreateItem([FromBody] ItemCreateDto itemDto)
    {
        _logger.LogInformation("Creating new item");
        var item = await _characterService.CreateItemAsync(itemDto);

        if (item == null)
        {
            _logger.LogError("Failed to create item");
            return BadRequest("Unable to create item.");
        }

        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
    }

    [HttpGet("items/{id}")]
    [Authorize(Roles = "GameMaster")]
    public async Task<IActionResult> GetItemById(int id)
    {
        _logger.LogInformation($"Getting item with ID {id}");
        var item = await _characterService.GetItemByIdAsync(id);

        if (item == null)
        {
            _logger.LogWarning($"Item with ID {id} not found");
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost("items/grant")]
    public async Task<IActionResult> GrantItem([FromBody] ItemGrantDto grantDto)
    {
        _logger.LogInformation($"Granting item {grantDto.ItemId} to character {grantDto.CharacterId}");
        var result = await _characterService.GrantItemAsync(grantDto);

        if (!result)
        {
            _logger.LogError($"Failed to grant item {grantDto.ItemId} to character {grantDto.CharacterId}");
            return BadRequest("Unable to grant item.");
        }

        return Ok();
    }

    [HttpPost("items/gift")]
    public async Task<IActionResult> GiftItem([FromBody] ItemGiftDto giftDto)
    {
        _logger.LogInformation($"Gifting item {giftDto.ItemId} from character {giftDto.FromCharacterId} to character {giftDto.ToCharacterId}");
        var result = await _characterService.GiftItemAsync(giftDto);

        if (!result)
        {
            _logger.LogError($"Failed to gift item {giftDto.ItemId} from character {giftDto.FromCharacterId} to character {giftDto.ToCharacterId}");
            return BadRequest("Unable to gift item.");
        }

        return Ok();
    }
}
