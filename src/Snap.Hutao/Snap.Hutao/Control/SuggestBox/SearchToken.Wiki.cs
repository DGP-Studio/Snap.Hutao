// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic.Frozen;
using Snap.Hutao.Model.Metadata.Converter;
using Windows.UI;

namespace Snap.Hutao.Control.SuggestBox;

internal partial class SearchToken
{
    private SearchTokenKind kind;
    private bool isKindInitialized;
    private object isKindInitializedLock = new();

    public SearchToken(string value, Uri? sideIconUri)
        : this(value)
    {
        SideIconUri = sideIconUri;
    }

    public Uri? SideIconUri { get; }

    public Uri? IconUri
    {
        get => Kind switch
        {
            SearchTokenKind.AssociationTypes => AssociationTypeIconConverter.AssociationTypeNameToIconUri(Value),
            SearchTokenKind.ElementNames => ElementNameIconConverter.ElementNameToIconUri(Value),
            SearchTokenKind.WeaponTypes => WeaponTypeIconConverter.WeaponTypeNameToIconUri(Value),
            _ => null,
        };
    }

    public Color? Quality
    {
        get => Kind switch
        {
            SearchTokenKind.ItemQualities => QualityColorConverter.QualityNameToColor(Value),
            _ => null,
        };
    }

    public SearchTokenKind Kind
    {
        get
        {
            return LazyInitializer.EnsureInitialized(ref kind, ref isKindInitialized, ref isKindInitializedLock, GetKind);

            SearchTokenKind GetKind()
            {
                if (IntrinsicFrozen.AssociationTypes.Contains(Value))
                {
                    return SearchTokenKind.AssociationTypes;
                }

                if (IntrinsicFrozen.BodyTypes.Contains(Value))
                {
                    return SearchTokenKind.BodyTypes;
                }

                if (IntrinsicFrozen.ElementNames.Contains(Value))
                {
                    return SearchTokenKind.ElementNames;
                }

                if (IntrinsicFrozen.FightProperties.Contains(Value))
                {
                    return SearchTokenKind.FightProperties;
                }

                if (IntrinsicFrozen.ItemQualities.Contains(Value))
                {
                    return SearchTokenKind.ItemQualities;
                }

                if (IntrinsicFrozen.WeaponTypes.Contains(Value))
                {
                    return SearchTokenKind.WeaponTypes;
                }

                return SearchTokenKind.Other;
            }
        }
    }
}
