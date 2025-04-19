namespace Game.Features.Identity.CreateUser;

public record CreateRequest(string Email,string Password,string ConfirmedPassword);
