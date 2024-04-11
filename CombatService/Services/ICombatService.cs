using CombatService.Models.DataTransferObjects;

namespace CombatService.Services;

public interface ICombatService
{
    Task<int?> CreateChallengeAsync(ChallengeDto challengeDto, int userId);
    Task<bool> AttackAsync(int duelId, int attackerId);
    Task<bool> CastSpellAsync(int duelId, int casterId);
    Task<bool> HealAsync(int duelId, int healerId);
}
