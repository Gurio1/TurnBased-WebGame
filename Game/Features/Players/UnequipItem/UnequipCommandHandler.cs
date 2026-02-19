using Game.Application.SharedKernel;
using Game.Contracts;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Persistence.Mongo;
using Game.Persistence.Requests;
using Game.Utilities;
using Game.Utilities.Extensions;
using MongoDB.Driver;

namespace Game.Features.Players.UnequipItem;

public sealed class UnequipCommandHandler : IRequestHandler<UnequipCommand, Result<PlayerViewModel>>
{
    private readonly IPlayerRepository playerRepository;
    private readonly UpdatePlayerAfterEquipmentInteraction updatePlayerService;
    private readonly UrlBuilder urlBuilder;
    
    public UnequipCommandHandler(IPlayerRepository playerRepository,
        UpdatePlayerAfterEquipmentInteraction updatePlayerService,
        UrlBuilder urlBuilder)
    {
        this.playerRepository = playerRepository;
        this.updatePlayerService = updatePlayerService;
        this.urlBuilder = urlBuilder;
    }
    
    public async Task<Result<PlayerViewModel>> Handle(UnequipCommand request, CancellationToken cancellationToken)
    {
        var getPlayerResult = await playerRepository.GetByIdWithAbilities(request.PlayerId, cancellationToken);
        
        if (getPlayerResult.IsFailure) return getPlayerResult.AsError<PlayerViewModel>();
        
        var player = getPlayerResult.Value;
        
        var equipResult = player.Unequip(request.EquipmentSlot);
        
        if (equipResult.IsFailure)
            return Result<PlayerViewModel>.CustomError(equipResult.Error);
        
        var updateResult = await updatePlayerService.Update(player, cancellationToken);
        
        return updateResult.IsFailure
            ? updateResult.AsError<PlayerViewModel>()
            : Result<PlayerViewModel>.Success(updateResult.Value.ToViewModel(urlBuilder));
    }
}
