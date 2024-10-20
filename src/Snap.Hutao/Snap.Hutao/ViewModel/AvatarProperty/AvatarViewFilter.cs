// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.AvatarProperty;

internal static class AvatarViewFilter
{
    public static Predicate<AvatarView> Compile(ObservableCollection<SearchToken> input)
    {
        return avatar => DoFilter(input, avatar);
    }

    private static bool DoFilter(ObservableCollection<SearchToken> input, AvatarView avatarView)
    {
        List<bool> matches = [];

        foreach ((SearchTokenKind kind, IEnumerable<string> tokens) in input.GroupBy(token => token.Kind, token => token.Value))
        {
            switch (kind)
            {
                case SearchTokenKind.ElementName:
                    if (IntrinsicFrozen.ElementNames.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatarView.Element.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                case SearchTokenKind.WeaponType:
                    if (IntrinsicFrozen.WeaponTypes.Overlaps(tokens))
                    {
                        ArgumentNullException.ThrowIfNull(avatarView.Weapon);
                        matches.Add(tokens.Contains(avatarView.Weapon.WeaponType.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                case SearchTokenKind.ItemQuality:
                    if (IntrinsicFrozen.ItemQualities.Overlaps(tokens))
                    {
                        matches.Add(tokens.Contains(avatarView.Quality.GetLocalizedDescriptionOrDefault()));
                    }

                    break;
                default:
                    matches.Add(false);
                    break;
            }
        }

        return matches.Count > 0 && matches.Aggregate((a, b) => a && b);
    }
}