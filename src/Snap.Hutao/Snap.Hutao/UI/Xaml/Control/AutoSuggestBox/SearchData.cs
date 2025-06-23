// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

internal sealed partial class SearchData : ObservableObject
{
    public SearchData(FrozenDictionary<string, SearchToken> availableTokens)
    {
        AvailableTokens = availableTokens;
    }

    public FrozenDictionary<string, SearchToken> AvailableTokens { get; }

    public ObservableCollection<SearchToken> FilterTokens { get; } = [];

    public string? FilterToken { get; set => SetProperty(ref field, value); }

    public static SearchData CreateForAvatarProperty()
    {
        return new(SearchTokens.GetForAvatarProperty());
    }

    public static SearchData CreateForWikiAvatar(ImmutableArray<Avatar> array)
    {
        return new(SearchTokens.GetForWikiAvatar(array));
    }

    public static SearchData CreateForWikiWeapon(ImmutableArray<Weapon> array)
    {
        return new(SearchTokens.GetForWikiWeapon(array));
    }
}