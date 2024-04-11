namespace CharacterService.Models.DataTransferObjects;

public class ItemCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int BonusStrength { get; set; }
    public int BonusAgility { get; set; }
    public int BonusIntelligence { get; set; }
    public int BonusFaith { get; set; }
}
