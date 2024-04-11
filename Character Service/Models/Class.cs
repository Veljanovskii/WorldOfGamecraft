namespace CharacterService.Models;

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<Character> Characters { get; set; }
}
