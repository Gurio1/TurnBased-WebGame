namespace Game.Features.Identity.Endpoints;

public record CreateRequest(string Email,string Password,string ConfirmedPassword);