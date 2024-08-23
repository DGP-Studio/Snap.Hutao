// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace Snap.Hutao.Service.AvatarInfo.Transformer;

[Injection(InjectAs.Transient)]
internal sealed class AvatarInfoDetailedCharacterTransformer : IDetailedCharacterTransformer<Web.Enka.Model.AvatarInfo>
{
    /// <inheritdoc/>
    public void Transform(ref readonly DetailedCharacter detailedCharacter, Web.Enka.Model.AvatarInfo source)
    {
        // update fetter
        detailedCharacter.FetterInfo ??= new();
        detailedCharacter.FetterInfo.ExpLevel = source.Fetter;

        // update level
        detailedCharacter.PropMap ??= [];
        detailedCharacter.PropMap[PlayerProperty.PROP_LEVEL] = new(PlayerProperty.PROP_LEVEL, source.Level);

        // update constellations
        detailedCharacter.TalentIdList = source.Constellations.Where(t => t.IsActived).Select(t => t.Id).ToList();

        // update relic
        detailedCharacter.EquipList ??= source.Reliquaries.SelectList(r => new Equip()
        {
            ItemId = r.Id,
            Reliquary = new() { Level = r.Level + 1, },
            Flat = new() { ItemType = ItemType.ITEM_RELIQUARY, EquipType = r.Position, },
        });

        if (detailedCharacter.EquipList.LastOrDefault() is null or { Weapon: null })
        {
            // 不存在武器则添加
            detailedCharacter.EquipList.Add(new());
        }

        Equip equip = detailedCharacter.EquipList.Last();

        // 切换了武器
        equip.ItemId = source.Weapon.Id;
        equip.Weapon = new()
        {
            Level = source.Weapon.Level,
            AffixMap = new()
            {
                [100000U + source.Weapon.Id] = source.Weapon.AffixLevel - 1,
            },
        };

        // Special case here, don't set EQUIP_WEAPON
        equip.Flat = new() { ItemType = ItemType.ITEM_WEAPON, };
    }
}