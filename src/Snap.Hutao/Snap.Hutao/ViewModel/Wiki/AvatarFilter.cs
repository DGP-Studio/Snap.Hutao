// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Wiki;

// ReSharper disable PossibleMultipleEnumeration
internal static class AvatarFilter
{
    public static Predicate<Avatar>? Compile(SearchData? searchData)
    {
        return searchData is { FilterTokens.Count: > 0 } ? Compile(searchData.FilterTokens) : default;
    }

    public static Predicate<Avatar> Compile(IEnumerable<SearchToken> input)
    {
        ILookup<SearchTokenKind, string> lookup = input.ToLookup(token => token.Kind, token => token.Value);
        return avatar => Compile(lookup, avatar);
    }

    private static bool Compile(ILookup<SearchTokenKind, string> lookup, Avatar avatar)
    {
        List<bool> matches = [];

        // Tokens is a BCL internal Grouping<string>, enumerating will use an internal PartialArrayEnumerator<string>
        foreach ((SearchTokenKind kind, IEnumerable<string> tokens) in lookup)
        {
            switch (kind)
            {
                case SearchTokenKind.ElementName:
                    if (IntrinsicFrozen.ElementNames.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.FetterInfo.VisionBefore));
                    }

                    break;
                case SearchTokenKind.AssociationType:
                    if (IntrinsicFrozen.AssociationTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.FetterInfo.Association.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.WeaponType:
                    if (IntrinsicFrozen.WeaponTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.Weapon.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.ItemQuality:
                    if (IntrinsicFrozen.ItemQualities.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.Quality.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    break;
                case SearchTokenKind.BodyType:
                    if (IntrinsicFrozen.BodyTypes.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatar.Body.GetLocalizedDescriptionOrDefault(SH.ResourceManager)));
                    }

                    break;
                case SearchTokenKind.Avatar:
                    matches.Add(tokens.Contains(avatar.Name));
                    break;
                default:
                    matches.Add(false);
                    break;
            }
        }

        return matches.Count > 0 && matches.All(r => r);
    }
}