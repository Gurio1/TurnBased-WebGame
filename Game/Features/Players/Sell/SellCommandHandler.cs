using Game.Application.SharedKernel;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Features.Players.Contracts;
using Game.Persistence.Requests;

namespace Game.Features.Players.Sell;

public sealed class SellCommandHandler : IRequestHandler<SellCommand,Result<PlayerViewModel>>
{
    private readonly IPlayerRepository playerRepository;
    private readonly UpdatePlayerAfterSellInteraction updateDef;
    
    public SellCommandHandler(IPlayerRepository playerRepository,UpdatePlayerAfterSellInteraction updateDef)
    {
        this.playerRepository = playerRepository;
        this.updateDef = updateDef;
    }
    public async Task<Result<PlayerViewModel>> Handle(SellCommand request, CancellationToken cancellationToken)
    {
        var getPlayerResult = await playerRepository.GetByIdWithAbilities(request.PlayerId, cancellationToken);
        
        if (getPlayerResult.IsFailure) return getPlayerResult.AsError<PlayerViewModel>();
        
        var player = getPlayerResult.Value;
        
        var equipResult = player.Sell(request.ItemId);
        
        if (equipResult.IsFailure) return Result<PlayerViewModel>.CustomError(equipResult.Error);
        
        var updateResult = await updateDef.Update(player, cancellationToken);
        
        return updateResult.IsFailure
            ? updateResult.AsError<PlayerViewModel>()
            : Result<PlayerViewModel>.Success(updateResult.Value.ToViewModel());
    }
}
