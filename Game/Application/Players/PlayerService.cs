using Game.Contracts;
using Game.Core.Equipment;
using Game.Core.Loot;
using Game.Core.Models;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Core.PlayerProfile.ValueObjects;
using Game.SharedKernel;
using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Models;
using Game.SharedKernel.Results;
using Game.Utilities;
using Game.Utilities.Extensions;

namespace Game.Application.Players;

public sealed class PlayerService(
    IPlayerRepository playerRepository,
    IMonsterRepository monsterRepository,
    ILootService lootService,
    UrlBuilder urlBuilder) : IPlayerService
{
    public Task<Result<GamePlayer>> GetById(string playerId, CancellationToken ct = default) =>
        playerRepository.GetById(playerId, ct);

    public async Task<Result<PlayerViewModel>> EquipItem(string playerId, string itemId, CancellationToken ct = default)
    {
        var playerResult = await playerRepository.GetById(playerId, ct);
        if (playerResult.IsFailure)
            return playerResult.AsError<PlayerViewModel>();

        var equipResult = playerResult.Value.Equip(itemId);
        if (equipResult.IsFailure)
            return Result<PlayerViewModel>.CustomError(equipResult.Error);

        return await SaveAndMap(playerResult.Value, ct);
    }

    public async Task<Result<PlayerViewModel>> SellItem(string playerId, string itemId, CancellationToken ct = default)
    {
        var playerResult = await playerRepository.GetById(playerId, ct);
        if (playerResult.IsFailure)
            return playerResult.AsError<PlayerViewModel>();

        var sellResult = playerResult.Value.Sell(itemId);
        if (sellResult.IsFailure)
            return Result<PlayerViewModel>.CustomError(sellResult.Error);

        return await SaveAndMap(playerResult.Value, ct);
    }

    public async Task<Result<PlayerViewModel>> UnequipItem(string playerId, string equipmentSlot, CancellationToken ct = default)
    {
        var playerResult = await playerRepository.GetById(playerId, ct);
        if (playerResult.IsFailure)
            return playerResult.AsError<PlayerViewModel>();

        var unequipResult = playerResult.Value.Unequip(equipmentSlot);
        if (unequipResult.IsFailure)
            return Result<PlayerViewModel>.CustomError(unequipResult.Error);

        return await SaveAndMap(playerResult.Value, ct);
    }

    public async Task<Result<string>> Create(CancellationToken ct = default)
    {
        var player = new GamePlayer
        {
            AbilityIds = ["0", "1"],
            Stats = new Stats
            {
                MaxHealth = 250,
                CriticalDamage = 30,
                CriticalChance = 10,
                Damage = 20f,
                CurrentHealth = 250f
            }
        };

        var createResult = await playerRepository.Create(player, ct);
        return createResult.IsFailure
            ? createResult.AsError<string>()
            : Result<string>.Success(createResult.Value.Id);
    }

    public Task<ResultWithoutValue> Delete(string playerId, CancellationToken ct = default) =>
        playerRepository.Delete(playerId, ct);

    public async Task<Result<BattleResolveResponse>> ResolveBattle(
        string playerId,
        BattleResolveRequest request,
        CancellationToken ct = default)
    {
        var playerResult = await playerRepository.GetById(playerId, ct);
        if (playerResult.IsFailure)
            return playerResult.AsError<BattleResolveResponse>();

        var player = playerResult.Value;
        player.Inventory.RemoveUsedItems(request.UsedItems);
        player.Stats = request.PlayerStats;

        BattleRewardDto? reward = null;

        if (request.Won)
        {
            reward = new BattleRewardDto
            {
                Gold = 20,
                Experience = 5
            };

            var monsterResult = await monsterRepository.GetByName(request.MonsterName, ct);
            if (monsterResult.IsFailure)
                return monsterResult.AsError<BattleResolveResponse>();

            var dropResult = await lootService.GenerateDrop(monsterResult.Value);
            if (dropResult.IsFailure)
                return dropResult.AsError<BattleResolveResponse>();

            var drop = dropResult.Value;
            if (drop is not null)
            {
                player.Inventory.Add(drop.Item, drop.Quantity);

                if (drop.Item is EquipmentBase equipment)
                {
                    reward.EquipmentDrop =
                    [
                        new BattleEquipmentDto
                        {
                            Id = equipment.Id,
                            Name = equipment.Name,
                            ImageUrl = equipment.ImageUrl,
                            EquipmentId = equipment.EquipmentId,
                            Slot = equipment.Slot
                        }
                    ];
                }
                else
                {
                    reward.Drop =
                    [
                        new BattleLootItemDto
                        {
                            Id = drop.Item.Id,
                            Name = drop.Item.Name,
                            ImageUrl = drop.Item.ImageUrl,
                            MaxInventorySlotQuantity = drop.Item.MaxInventorySlotQuantity,
                            Quantity = drop.Quantity
                        }
                    ];
                }
            }
        }

        var saveResult = await playerRepository.Save(player, ct);
        return saveResult.IsFailure
            ? saveResult.AsError<BattleResolveResponse>()
            : Result<BattleResolveResponse>.Success(new BattleResolveResponse { Reward = reward });
    }

    private async Task<Result<PlayerViewModel>> SaveAndMap(GamePlayer player, CancellationToken ct)
    {
        var saveResult = await playerRepository.Save(player, ct);
        return saveResult.IsFailure
            ? saveResult.AsError<PlayerViewModel>()
            : Result<PlayerViewModel>.Success(saveResult.Value.ToViewModel(urlBuilder));
    }
}
