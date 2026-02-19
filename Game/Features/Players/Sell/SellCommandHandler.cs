using Game.Application.SharedKernel;
using Game.Contracts;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Persistence.Requests;
using Game.Utilities;
using Game.Utilities.Extensions;

namespace Game.Features.Players.Sell;

public sealed class SellCommandHandler : IRequestHandler<SellCommand,Result<PlayerViewModel>>
{
    private readonly IPlayerRepository playerRepository;
    private readonly UpdatePlayerAfterSellInteraction updateDef;
    private readonly UrlBuilder urlBuilder;
    
    public SellCommandHandler(IPlayerRepository playerRepository,UpdatePlayerAfterSellInteraction updateDef, UrlBuilder urlBuilder)
    {
        this.playerRepository = playerRepository;
        this.updateDef = updateDef;
        this.urlBuilder = urlBuilder;
    }
    public async Task<Result<PlayerViewModel>> Handle(SellCommand request, CancellationToken cancellationToken)
    {
        var getPlayerResult = await playerRepository.GetByIdWithAbilities(request.PlayerId, cancellationToken);
        
        if (getPlayerResult.IsFailure) return getPlayerResult.AsError<PlayerViewModel>();
        
        var player = getPlayerResult.Value;
        
        var sellResult = player.Sell(request.ItemId);
        
        if (sellResult.IsFailure) return Result<PlayerViewModel>.CustomError(sellResult.Error);
        
        var updateResult = await updateDef.Update(player, cancellationToken);
        
        return updateResult.IsFailure
            ? updateResult.AsError<PlayerViewModel>()
            : Result<PlayerViewModel>.Success(updateResult.Value.ToViewModel(urlBuilder));
    }
}
