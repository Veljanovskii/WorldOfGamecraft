namespace CharacterService.Models.DataTransferObjects;

public class CharacterCreateDto
{
    public string Name { get; set; }
    public int ClassId { get; set; }
    public int Health { get; set; }
    public int Mana { get; set; }
    public int BaseStrength { get; set; }
    public int BaseAgility { get; set; }
    public int BaseIntelligence { get; set; }
    public int BaseFaith { get; set; }
}
