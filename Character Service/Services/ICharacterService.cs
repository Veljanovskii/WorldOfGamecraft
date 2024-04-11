using CharacterService.Models.DataTransferObjects;

namespace CharacterService.Services;

public interface ICharacterService
{
    Task<IEnumerable<CharacterDto>> GetCharactersAsync();
    Task<CharacterDetailDto> GetCharacterByIdAsync(int id);
    Task<CharacterDetailDto> CreateCharacterAsync(CharacterCreateDto characterDto, int createdById);
    Task<IEnumerable<ItemDto>> GetItemsAsync();
    Task<ItemDto> CreateItemAsync(ItemCreateDto itemDto);
    Task<ItemDto> GetItemByIdAsync(int id);
    Task<bool> GrantItemAsync(ItemGrantDto grantDto);
    Task<bool> GiftItemAsync(ItemGiftDto giftDto);
}
