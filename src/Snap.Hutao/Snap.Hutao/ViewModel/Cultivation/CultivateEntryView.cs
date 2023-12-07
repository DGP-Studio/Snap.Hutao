// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using System.Text;

namespace Snap.Hutao.ViewModel.Cultivation;

/// <summary>
/// 养成物品
/// </summary>
[HighQuality]
internal sealed class CultivateEntryView : Item
{
    public CultivateEntryView(CultivateEntry entry, Item item, List<CultivateItemView> items)
    {
        Id = entry.Id;
        EntryId = entry.InnerId;
        Name = item.Name;
        Icon = item.Icon;
        Badge = item.Badge;
        Quality = item.Quality;
        Items = items;

        Description = ParseDescription(entry);
        IsToday = items.Any(i => i.IsToday);
        DaysOfWeek = items.FirstOrDefault(i => i.DaysOfWeek != DaysOfWeek.Any)?.DaysOfWeek ?? DaysOfWeek.Any;

        static string ParseDescription(CultivateEntry entry)
        {
            if (entry.LevelInformation is null)
            {
                return SH.ViewModelCultivationEntryViewDescriptionDefault;
            }

            CultivateEntryLevelInformation info = entry.LevelInformation;

            switch (entry.Type)
            {
                case Model.Entity.Primitive.CultivateType.AvatarAndSkill:
                    {
                        StringBuilder stringBuilder = new();

                        if (info.AvatarLevelFrom != info.AvatarLevelTo)
                        {
                            stringBuilder.Append("Lv.").Append(info.AvatarLevelFrom).Append(" → Lv.").Append(info.AvatarLevelTo).Append(' ');
                        }

                        if (info.SkillALevelFrom != info.SkillALevelTo)
                        {
                            stringBuilder.Append("A: ").Append(info.SkillALevelFrom).Append(" → ").Append(info.SkillALevelTo).Append(' ');
                        }

                        if (info.SkillELevelFrom != info.SkillELevelTo)
                        {
                            stringBuilder.Append("E: ").Append(info.SkillELevelFrom).Append(" → ").Append(info.SkillELevelTo).Append(' ');
                        }

                        if (info.SkillQLevelFrom != info.SkillQLevelTo)
                        {
                            stringBuilder.Append("Q: ").Append(info.SkillQLevelFrom).Append(" → ").Append(info.SkillQLevelTo).Append(' ');
                        }

                        return stringBuilder.ToStringTrimEnd();
                    }

                case Model.Entity.Primitive.CultivateType.Weapon:
                    {
                        StringBuilder stringBuilder = new();

                        if (info.WeaponLevelFrom != info.WeaponLevelTo)
                        {
                            stringBuilder.Append("Lv.").Append(info.WeaponLevelFrom).Append(" → Lv.").Append(info.WeaponLevelTo);
                        }

                        return stringBuilder.ToString();
                    }
            }

            return string.Empty;
        }
    }

    public List<CultivateItemView> Items { get; set; } = default!;

    public bool IsToday { get; }

    public DaysOfWeek DaysOfWeek { get; }

    public string Description { get; }

    internal uint Id { get; set; }

    internal Guid EntryId { get; set; }
}