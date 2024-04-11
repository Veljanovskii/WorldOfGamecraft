namespace CombatService.Models;

public class DuelAction
{
    public int Id { get; set; }
    public int DuelId { get; set; }
    public DuelActionType ActionType { get; set; }
    public int CharacterId { get; set; }
    public DateTime Timestamp { get; set; }
    // Other properties...
}
