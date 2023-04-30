// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace Snap.Hutao.Service.AvatarInfo.Transformer;

/// <summary>
/// 游戏记录角色转角色详情转换器
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient)]
internal sealed class GameRecordCharacterAvatarInfoTransformer : IAvatarInfoTransformer<Character>
{
    /// <summary>
    /// 构造一个新的游戏记录角色转角色详情转换器
    /// </summary>
    public GameRecordCharacterAvatarInfoTransformer()
    {
    }

    /// <summary>
    /// Id 角色映射
    /// </summary>
    public Dictionary<AvatarId, Model.Metadata.Avatar.Avatar>? IdAvatarMap { get; set; }

    /// <inheritdoc/>
    public void Transform(ref Web.Enka.Model.AvatarInfo avatarInfo, Character source)
    {
        Model.Metadata.Avatar.Avatar avatar = Must.NotNull(IdAvatarMap!)[source.Id];

        // update fetter
        avatarInfo.FetterInfo ??= new();
        avatarInfo.FetterInfo.ExpLevel = source.Fetter;

        // update level
        avatarInfo.PropMap ??= new Dictionary<PlayerProperty, TypeValue>();
        avatarInfo.PropMap[PlayerProperty.PROP_LEVEL] = new(PlayerProperty.PROP_LEVEL, source.Level);

        // update constellations
        avatarInfo.TalentIdList = source.Constellations.Where(t => t.IsActived).Select(t => t.Id).ToList();

        // update relic
        avatarInfo.EquipList ??= source.Reliquaries.SelectList(r => new Equip()
        {
            ItemId = r.Id,
            Reliquary = new() { Level = r.Level + 1, },
            Flat = new() { ItemType = ItemType.ITEM_RELIQUARY, EquipType = r.Position, },
        });

        Equip? equipTest = avatarInfo.EquipList.LastOrDefault();
        if (equipTest == null || equipTest.Weapon == null)
        {
            // 不存在武器则添加
            avatarInfo.EquipList.Add(new());
        }

        Equip equip = avatarInfo.EquipList.Last();

        equip.ItemId = source.Weapon.Id;
        equip.Weapon = new()
        {
            Level = source.Weapon.Level,
            AffixMap = new() { { $"1{source.Weapon.Id}", source.Weapon.AffixLevel - 1 }, },
        };

        // Special case here, don't set EQUIP_WEAPON
        equip.Flat = new() { ItemType = ItemType.ITEM_WEAPON, };
    }
}