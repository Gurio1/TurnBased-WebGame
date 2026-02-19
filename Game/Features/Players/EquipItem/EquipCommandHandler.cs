using Game.Application.SharedKernel;
using Game.Contracts;
using Game.Core.PlayerProfile;
using Game.Persistence.Requests;
using Game.Utilities;
using Game.Utilities.Extensions;

namespace Game.Features.Players.EquipItem;

public sealed class EquipCommandHandler : IRequestHandler<EquipCommand, Result<PlayerViewModel>>
{
    private readonly IPlayerRepository playerRepository;
    private readonly UpdatePlayerAfterEquipmentInteraction updatePlayerService;
    private readonly UrlBuilder urlBuilder;
    
    public EquipCommandHandler(IPlayerRepository playerRepository,
        UpdatePlayerAfterEquipmentInteraction updatePlayerService,
        UrlBuilder urlBuilder)
    {
        this.playerRepository = playerRepository;
        this.updatePlayerService = updatePlayerService;
        this.urlBuilder = urlBuilder;
    }
    
    public async Task<Result<PlayerViewModel>> Handle(EquipCommand request, CancellationToken cancellationToken)
    {
        var getPlayerResult = await playerRepository.GetByIdWithAbilities(request.PlayerId, cancellationToken);
        
        if (getPlayerResult.IsFailure) return getPlayerResult.AsError<PlayerViewModel>();
        
        var player = getPlayerResult.Value;
        
        var equipResult = player.Equip(request.ItemId);
        
        if (equipResult.IsFailure) return Result<PlayerViewModel>.CustomError(equipResult.Error);
        
        var updateResult = await updatePlayerService.Update(player, cancellationToken);
        
        return updateResult.IsFailure
            ? updateResult.AsError<PlayerViewModel>()
            : Result<PlayerViewModel>.Success(updateResult.Value.ToViewModel(urlBuilder));
    }
}
