namespace CombatService.Models;

public class Duel
{
    public int Id { get; set; }
    public int ChallengerId { get; set; }
    public int ChallengeeId { get; set; }
    public DateTime StartTime { get; set; }
    public DuelStatus Status { get; set; }
    public List<DuelAction> DuelActions { get; set; }
}
