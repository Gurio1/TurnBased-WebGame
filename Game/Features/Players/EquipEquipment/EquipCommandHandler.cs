using Game.Core.SharedKernel;
using Game.Features.Players.Contracts;
using Game.Persistence.Repositories;
using Game.Persistence.Requests;

namespace Game.Features.Players.EquipEquipment;

//TODO: I dont know.Should i create different commands for getting and updating a player?(This operations are too simple so i dont see any reason)
public sealed class EquipCommandHandler : IRequestHandler<EquipCommand, Result<PlayerViewModel>>
{
    private readonly IPlayerRepository playerRepository;
    private readonly UpdatePlayerAfterEquipmentInteraction updatePlayerService;
    
    public EquipCommandHandler(IPlayerRepository playerRepository, UpdatePlayerAfterEquipmentInteraction updatePlayerService)
    {
        this.playerRepository = playerRepository;
        this.updatePlayerService = updatePlayerService;
    }
    
    public async Task<Result<PlayerViewModel>> Handle(EquipCommand request, CancellationToken cancellationToken)
    {
        var getPlayerResult = await playerRepository.GetByIdWithAbilities(request.PlayerId, cancellationToken);;
        
        if (getPlayerResult.IsFailure)
        {
            return getPlayerResult.AsError<PlayerViewModel>();
        }
        
        var player = getPlayerResult.Value;
        
        var equipResult = player.Equip(request.ItemId);
        
        if (equipResult.IsFailure)
        {
            return Result<PlayerViewModel>.CustomError(equipResult.Error);
        }
        
        var updateResult = await updatePlayerService.Update(player, cancellationToken);
        
        return updateResult.IsFailure
            ? updateResult.AsError<PlayerViewModel>()
            : Result<PlayerViewModel>.Success(updateResult.Value.ToViewModel());
    }
}
