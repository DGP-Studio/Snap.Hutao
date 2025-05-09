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
                // {weapon.Name} [{weapon.Level}, ☆{weapon.Quality:D}, R{weapon.AffixLevelNumber}]
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
        foreach (ref readonly AvatarProperty property in properties.AsSpan())
        {
            result.Append("// [").Append(property.Name).Append(": ").Append(property.Value).Append(']').AppendLine();
        }

        return result.ToString();
    }

    [SuppressMessage("", "CA1305")]
    private static string FormatReliquaries(ImmutableArray<ReliquaryView> reliquaries)
    {
        StringBuilder result = new();
        foreach (ref readonly ReliquaryView reliquary in reliquaries.AsSpan())
        {
            NameValue<string>? mainProperty = reliquary.MainProperty;
            result.Append($"""
                    // {ReliquaryEmoji(reliquary.EquipType)} {mainProperty?.Name}: {mainProperty?.Value} [☆{reliquary.Quality:D} {reliquary.Level} {reliquary.SetName}]

                    """);
            result.Append("// ");

            foreach (ref readonly ReliquaryComposedSubProperty subProperty in reliquary.ComposedSubProperties.AsSpan())
            {
                result.Append('[').Append(subProperty.Name).Append(": ").Append(subProperty.Value).Append(']');
            }

            result.AppendLine();
        }

        return result.ToString();
    }

    private static string ReliquaryEmoji(EquipType type)
    {
        return type switch
        {
            EquipType.EQUIP_BRACER => "🌷",
            EquipType.EQUIP_NECKLACE => "🪶",
            EquipType.EQUIP_SHOES => "⏳",
            EquipType.EQUIP_RING => "🍷",
            EquipType.EQUIP_DRESS => "👑",
            _ => string.Empty,
        };
    }
}