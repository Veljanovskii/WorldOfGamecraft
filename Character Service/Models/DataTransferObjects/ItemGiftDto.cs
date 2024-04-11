namespace CharacterService.Models.DataTransferObjects;

public class ItemGiftDto
{
    public int FromCharacterId { get; set; }
    public int ToCharacterId { get; set; }
    public int ItemId { get; set; }
}
