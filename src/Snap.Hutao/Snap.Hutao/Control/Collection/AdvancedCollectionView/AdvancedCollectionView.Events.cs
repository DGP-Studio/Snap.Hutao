// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Data;
using Windows.Foundation.Collections;

namespace Snap.Hutao.Control.Collection.AdvancedCollectionView;

/// <summary>
/// A collection view implementation that supports filtering, grouping, sorting and incremental loading
/// </summary>
internal partial class AdvancedCollectionView
{
    /// <summary>
    /// Currently selected item changing event
    /// </summary>
    /// <param name="e">event args</param>
    private void OnCurrentChanging(CurrentChangingEventArgs e)
    {
        if (_deferCounter > 0)
        {
            return;
        }

        CurrentChanging?.Invoke(this, e);
    }

    /// <summary>
    /// Currently selected item changed event
    /// </summary>
    /// <param name="e">event args</param>
    private void OnCurrentChanged(object e)
    {
        if (_deferCounter > 0)
        {
            return;
        }

        CurrentChanged?.Invoke(this, e);

        // ReSharper disable once ExplicitCallerInfoArgument
        OnPropertyChanged(nameof(CurrentItem));
    }

    /// <summary>
    /// Vector changed event
    /// </summary>
    /// <param name="e">event args</param>
    private void OnVectorChanged(IVectorChangedEventArgs e)
    {
        if (_deferCounter > 0)
        {
            return;
        }

        VectorChanged?.Invoke(this, e);

        // ReSharper disable once ExplicitCallerInfoArgument
        OnPropertyChanged(nameof(Count));
    }
}
