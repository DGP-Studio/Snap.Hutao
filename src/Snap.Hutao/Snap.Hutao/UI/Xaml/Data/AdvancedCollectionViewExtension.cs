// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Data;

internal static class AdvancedCollectionViewExtension
{
    public static void MoveCurrentToFirstOrDefault<T>(this IAdvancedCollectionView<T> view)
        where T : class
    {
        if (!view.MoveCurrentToFirst())
        {
            view.MoveCurrentTo(default);
        }
    }
}