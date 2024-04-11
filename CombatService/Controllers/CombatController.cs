using CombatService.Models.DataTransferObjects;
using CombatService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CombatService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CombatController(ICombatService combatService,
    ILogger<CombatController> logger) : ControllerBase
{
    private readonly ICombatService _combatService = combatService;
    private readonly ILogger<CombatController> _logger = logger;

    [HttpPost("challenge")]
    public async Task<IActionResult> CreateChallenge([FromBody] ChallengeDto challengeDto)
    {
        _logger.LogInformation("Creating a new challenge");
        // Extract user ID from the JWT token
        /*var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }
        var duelId = await _combatService.CreateChallengeAsync(challengeDto, int.Parse(userId));*/

        var duelId = await _combatService.CreateChallengeAsync(challengeDto, 1);

        if (!duelId.HasValue)
        {
            _logger.LogWarning("Failed to create challenge");
            return BadRequest("Unable to create challenge.");
        }

        return Ok(new { DuelId = duelId.Value });
    }

    [HttpPost("{duelId}/attack")]
    public async Task<IActionResult> Attack(int duelId)
    {
        _logger.LogInformation($"Attacking in duel {duelId}");
        // Extract user ID from the JWT token
        /*var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }
        var result = await _combatService.AttackAsync(duelId, userId);*/

        var result = await _combatService.AttackAsync(duelId, 1);

        if (!result)
        {
            _logger.LogWarning($"Failed to attack in duel {duelId}");
            return BadRequest("Unable to perform attack.");
        }

        return Ok();
    }

    [HttpPost("{duelId}/cast")]
    public async Task<IActionResult> Cast(int duelId)
    {
        _logger.LogInformation($"Casting spell in duel {duelId}");
        // Extract user ID from the JWT token
        /*var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }
        var result = await _combatService.CastSpellAsync(duelId, userId);*/

        var result = await _combatService.CastSpellAsync(duelId, 1);

        if (!result)
        {
            _logger.LogWarning($"Failed to cast spell in duel {duelId}");
            return BadRequest("Unable to cast spell.");
        }

        return Ok();
    }

    [HttpPost("{duelId}/heal")]
    public async Task<IActionResult> Heal(int duelId)
    {
        _logger.LogInformation($"Healing in duel {duelId}");
        // Extract user ID from the JWT token
        /*var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }
        var result = await _combatService.HealAsync(duelId, userId);*/

        var result = await _combatService.HealAsync(duelId, 1);

        if (!result)
        {
            _logger.LogWarning($"Failed to heal in duel {duelId}");
            return BadRequest("Unable to heal.");
        }

        return Ok();
    }
}
