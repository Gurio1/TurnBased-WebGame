using Game.Contracts;
using Game.Core.Models;

namespace Game.Utilities.Extensions;

public static class ItemActionPresenter
{
    
    //TODO : Not very cool, but for now ok
    public static T WithActionsFrom<T>(this T dto, Item item, UrlBuilder urls)
        where T : ItemViewModel
    {
        var actions = new List<ItemActionDto>();
        
        if (item is IEquippable)
        {
            actions.Add(new ItemActionDto
            {
                Key = "Equip",
                Href = urls.Equip(item.Id),
                Method = "POST"
            });
        }
        
        if (item is ISellable)
        {
            actions.Add(new ItemActionDto
            {
                Key = "Sell",
                Href = urls.Sell(item.Id),
                Method = "POST"
            });
        }
        
        dto.ItemActions.AddRange(actions);
        return dto;
    }
}
