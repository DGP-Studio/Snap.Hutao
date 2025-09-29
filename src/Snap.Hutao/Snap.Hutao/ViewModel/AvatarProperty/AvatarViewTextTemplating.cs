// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;
using System.Text;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal static class AvatarViewTextTemplating
{
    public static string GetTemplatedText(AvatarView avatar)
    {
        string avatarTemplate = $"""
            // {avatar.Name} [{avatar.Level}, ☆{avatar.Quality:D}, C{avatar.Constellations.Count(c => c.IsActivated)}] [{FormatSkills(avatar.Skills)}]

            """;

        string weaponTemplate = avatar.Weapon is { } weapon
            ? $"""
                // ---------------------
                // {weapon.Name} [{weapon.Level}, ☆{weapon.Quality:D}, ❖{weapon.AffixLevelNumber}]
                // {(weapon.MainProperty is null ? string.Empty : $"[{weapon.MainProperty.Name}: {weapon.MainProperty.Value}]")} {(weapon.SubProperty is null ? string.Empty : $"[{weapon.SubProperty.Name}: {weapon.SubProperty.Value}]")}

                """
            : string.Empty;

        string propertiesTemplate = avatar.Properties.Length > 0
            ? $"""
                // ---------------------
                {FormatProperties(avatar.Properties)}
                """
            : string.Empty;

        string reliquariesTemplate = avatar.Reliquaries.Length > 0
            ? $"""
                // ---------------------
                {FormatReliquaries(avatar.Reliquaries)}
                """
            : string.Empty;

        return $"""
            // =====================
            {avatarTemplate}{weaponTemplate}{propertiesTemplate}{reliquariesTemplate}// =====================
            """;
    }

    private static string FormatSkills(ImmutableArray<SkillView> skills)
    {
        StringBuilder result = new();
        ReadOnlySpan<SkillView> skillSpan = skills.AsSpan();
        for (int index = 0; index < skillSpan.Length; index++)
        {
            ref readonly SkillView skill = ref skillSpan[index];
            result.Append(skill.Level);
            if (index < skillSpan.Length - 1)
            {
                result.Append(", ");
            }
        }

        return result.ToString();
    }

    private static string FormatProperties(ImmutableArray<AvatarProperty> properties)
    {
        StringBuilder result = new();
        foreach (AvatarProperty property in properties)
        {
            result.Append("// [").Append(property.Name).Append(": ").Append(property.Value);
            if (!string.IsNullOrEmpty(property.AddValue))
            {
                result.Append(" + ").Append(property.AddValue);
            }

            result.Append(']').AppendLine();
        }

        return result.ToString();
    }

    [SuppressMessage("", "CA1305")]
    private static string FormatReliquaries(ImmutableArray<ReliquaryView> reliquaries)
    {
        StringBuilder result = new();
        foreach (ReliquaryView reliquary in reliquaries)
        {
            NameValue<string>? mainProperty = reliquary.MainProperty;
            result.Append($"""
                    // {ReliquaryEmoji(reliquary.EquipType)} {mainProperty?.Name}: {mainProperty?.Value} [☆{reliquary.Quality:D} {reliquary.Level} {reliquary.SetName}]

                    """);
            result.Append("// ");

            foreach (ReliquaryComposedSubProperty subProperty in reliquary.ComposedSubProperties)
            {
                result.Append('[').Append(subProperty.Name).Append(": ").Append(subProperty.Value).Append(' ').Append((char)('\u2775' + subProperty.EnhancedCount)).Append(']');
            }

            result.AppendLine();
        }

        return result.ToString();
    }

    private static string ReliquaryEmoji(EquipType type)
    {
        return type switch
        {
            EquipType.EQUIP_BRACER => "\ud83c\udf37",   // 🌷
            EquipType.EQUIP_NECKLACE => "\ud83e\udeb6", // 🪶
            EquipType.EQUIP_SHOES => "\u23f3",          // ⏳
            EquipType.EQUIP_RING => "\ud83c\udf77",     // 🍷
            EquipType.EQUIP_DRESS => "\ud83d\udc51",    // 👑
            _ => string.Empty,
        };
    }
}