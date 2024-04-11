namespace CharacterService.Models;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Health { get; set; }
    public int Mana { get; set; }
    public int BaseStrength { get; set; }
    public int BaseAgility { get; set; }
    public int BaseIntelligence { get; set; }
    public int BaseFaith { get; set; }
    public int ClassId { get; set; }
    public Class Class { get; set; }
    public ICollection<Item> Items { get; set; }
    public int CreatedById { get; set; }
}
