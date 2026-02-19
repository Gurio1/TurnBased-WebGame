namespace Game.Utilities;

public sealed class UrlBuilder(LinkGenerator links, IHttpContextAccessor http)
{
    public string Equip(string itemId) =>
        links.GetUriByName(http.HttpContext!, EndpointNames.EquipItemEndpoint,   new { itemId })!;
    
    public string Sell(string itemId) =>
        links.GetUriByName(http.HttpContext!, EndpointNames.SellItemEndpoint,   new { itemId })!;
}
