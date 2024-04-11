namespace CharacterService.Models.DataTransferObjects;

public class CharacterDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Health { get; set; }
    public int Mana { get; set; }
    public int BaseStrength { get; set; }
    public int BaseAgility { get; set; }
    public int BaseIntelligence { get; set; }
    public int BaseFaith { get; set; }
    public int TotalStrength { get; set; }
    public int TotalAgility { get; set; }
    public int TotalIntelligence { get; set; }
    public int TotalFaith { get; set; }
    public List<ItemDto> Items { get; set; }
    public int CreatedById { get; set; }
}
