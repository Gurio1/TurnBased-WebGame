namespace Game.Utilities;

public sealed class UrlBuilder(LinkGenerator links)
{
    public string Equip(string itemId) =>
        links.GetPathByName(EndpointNames.EquipItemEndpoint, new { itemId })!;
    
    public string Sell(string itemId) =>
        links.GetPathByName(EndpointNames.SellItemEndpoint, new { itemId })!;
}
