using Game.Application.SharedKernel;
using Game.Core.PlayerProfile;
using Game.Features.Players.Contracts;
using Game.Persistence.Mongo;
using Game.Persistence.Requests;
using MongoDB.Driver;

namespace Game.Features.Players.UnequipEquipment;

public sealed class UnequipCommandHandler : IRequestHandler<UnequipCommand, Result<PlayerViewModel>>
{
    private readonly IMongoCollection<Player> collection;
    private readonly UpdatePlayerAfterEquipmentInteraction updatePlayerService;
    
    public UnequipCommandHandler(IMongoCollectionProvider collectionProvider,
        UpdatePlayerAfterEquipmentInteraction updatePlayerService)
    {
        this.updatePlayerService = updatePlayerService;
        collection = collectionProvider.GetCollection<Player>();
    }
    
    public async Task<Result<PlayerViewModel>> Handle(UnequipCommand request, CancellationToken cancellationToken)
    {
        var player = await collection.Find(p => p.Id == request.PlayerId).FirstOrDefaultAsync(cancellationToken);
        
        if (player is null)
            return Result<PlayerViewModel>.NotFound($"Player with id '{request.PlayerId}' was not found");
        
        var equipResult = player.Unequip(request.EquipmentSlot);
        
        if (equipResult.IsFailure)
            return Result<PlayerViewModel>.CustomError(equipResult.Error);
        
        var updateResult = await updatePlayerService.Update(player, cancellationToken);
        
        return updateResult.IsFailure
            ? updateResult.AsError<PlayerViewModel>()
            : Result<PlayerViewModel>.Success(updateResult.Value.ToViewModel());
    }
}
