// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace Snap.Hutao.Service.AvatarInfo.Composer;

/// <summary>
/// 游戏记录角色转角色详情转换器
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient)]
internal sealed class GameRecordCharacterAvatarInfoComposer : IAvatarInfoComposer<Character>
{
    private readonly IMetadataService metadataService;

    /// <summary>
    /// 构造一个新的游戏记录角色转角色详情转换器
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    public GameRecordCharacterAvatarInfoComposer(IMetadataService metadataService)
    {
        this.metadataService = metadataService;
    }

    /// <inheritdoc/>
    public async ValueTask<Web.Enka.Model.AvatarInfo> ComposeAsync(Web.Enka.Model.AvatarInfo avatarInfo, Character source)
    {
        Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> map = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
        Model.Metadata.Avatar.Avatar avatar = map[source.Id];

        // update fetter
        avatarInfo.FetterInfo ??= new();
        avatarInfo.FetterInfo.ExpLevel = source.Fetter;

        // update level
        avatarInfo.PropMap ??= new Dictionary<PlayerProperty, TypeValue>();
        avatarInfo.PropMap[PlayerProperty.PROP_LEVEL] = new(PlayerProperty.PROP_LEVEL, source.Level.ToString());

        // update constellations
        avatarInfo.TalentIdList = source.Constellations.Where(t => t.IsActived).Select(t => t.Id).ToList();

        // update relic
        avatarInfo.EquipList ??= source.Reliquaries.SelectList(r => new Equip()
        {
            ItemId = r.Id,
            Reliquary = new() { Level = r.Level + 1, },
            Flat = new() { ItemType = ItemType.ITEM_RELIQUARY, EquipType = r.Position, },
        });

        Equip? equip = avatarInfo.EquipList.LastOrDefault();
        if (equip == null || equip.Weapon == null)
        {
            // 不存在武器则添加
            avatarInfo.EquipList.Add(new());
        }

        equip = avatarInfo.EquipList.Last();

        equip.ItemId = source.Weapon.Id;
        equip.Weapon = new()
        {
            Level = source.Weapon.Level,
            AffixMap = new() { { $"1{source.Weapon.Id}", source.Weapon.AffixLevel - 1 }, },
        };

        // Special case here, don't set EQUIP_WEAPON
        equip.Flat = new() { ItemType = ItemType.ITEM_WEAPON, };

        return avatarInfo;
    }
}