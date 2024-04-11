using CombatService.Data;
using CombatService.Models;
using CombatService.Models.DataTransferObjects;

namespace CombatService.Services;

public class CombatService(CombatDbContext context) : ICombatService
{
    private readonly CombatDbContext _context = context;

    public async Task<int?> CreateChallengeAsync(ChallengeDto challengeDto, int userId)
    {
        var duel = new Duel
        {
            ChallengerId = challengeDto.ChallengerId,
            ChallengeeId = challengeDto.ChallengeeId,
            StartTime = DateTime.UtcNow,
            Status = DuelStatus.Ongoing
        };

        _context.Duels.Add(duel);
        await _context.SaveChangesAsync();

        return duel.Id;
    }

    public async Task<bool> AttackAsync(int duelId, int attackerId)
    {
        var duel = await _context.Duels.FindAsync(duelId);
        if (duel == null || (duel.ChallengerId != attackerId && duel.ChallengeeId != attackerId))
        {
            return false; // Duel not found or user not part of the duel
        }

        // Get the defender's ID
        var defenderId = duel.ChallengerId == attackerId ? duel.ChallengeeId : duel.ChallengerId;

        // Retrieve character stats (you might need to call the Character Service for this)
        var attackerStats = await GetCharacterStats(attackerId);
        var defenderStats = await GetCharacterStats(defenderId);

        // Calculate damage
        var damage = attackerStats.Strength + attackerStats.Agility;

        // Apply damage
        defenderStats.Health -= damage;

        // Update defender's health in the database or cache

        // Add an attack action to the duel
        var duelAction = new DuelAction
        {
            DuelId = duelId,
            ActionType = DuelActionType.Attack,
            CharacterId = attackerId,
            Timestamp = DateTime.UtcNow
        };
        _context.DuelActions.Add(duelAction);
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<CharacterStatsDto> GetCharacterStats(int characterId)
    {
        // Placeholder for fetching character stats from the Character Service

        return new CharacterStatsDto
        {
            Strength = 10,
            Agility = 5,
            Health = 100
        };
    }

    public async Task<bool> CastSpellAsync(int duelId, int casterId)
    {
        var duel = await _context.Duels.FindAsync(duelId);
        if (duel == null || (duel.ChallengerId != casterId && duel.ChallengeeId != casterId))
        {
            return false; // Duel not found or user not part of the duel
        }

        // Get the defender's ID
        var defenderId = duel.ChallengerId == casterId ? duel.ChallengeeId : duel.ChallengerId;

        // Retrieve character stats
        var casterStats = await GetCharacterStats(casterId);
        var defenderStats = await GetCharacterStats(defenderId);

        // Calculate spell damage
        var damage = 2 * casterStats.Intelligence;

        // Apply damage
        defenderStats.Health -= damage;

        // Update defender's health in the database or cache

        // Add a cast spell action to the duel
        var duelAction = new DuelAction
        {
            DuelId = duelId,
            ActionType = DuelActionType.Cast,
            CharacterId = casterId,
            Timestamp = DateTime.UtcNow
        };
        _context.DuelActions.Add(duelAction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> HealAsync(int duelId, int healerId)
    {
        var duel = await _context.Duels.FindAsync(duelId);
        if (duel == null || (duel.ChallengerId != healerId && duel.ChallengeeId != healerId))
        {
            return false; // Duel not found or user not part of the duel
        }

        // Retrieve character stats
        var healerStats = await GetCharacterStats(healerId);

        // Calculate healing amount
        var healingAmount = healerStats.Faith;

        // Apply healing
        healerStats.Health += healingAmount;

        // Update healer's health in the database or cache

        // Add a heal action to the duel
        var duelAction = new DuelAction
        {
            DuelId = duelId,
            ActionType = DuelActionType.Heal,
            CharacterId = healerId,
            Timestamp = DateTime.UtcNow
        };
        _context.DuelActions.Add(duelAction);
        await _context.SaveChangesAsync();

        return true;
    }
}
