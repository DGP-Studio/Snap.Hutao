// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Collection.AdvancedCollectionView;

/// <summary>
/// A collection view implementation that supports filtering, grouping, sorting and incremental loading
/// </summary>
internal partial class AdvancedCollectionView
{
    /// <summary>
    /// Stops refreshing until it is disposed
    /// </summary>
    /// <returns>An disposable object</returns>
    public IDisposable DeferRefresh()
    {
        return new NotificationDeferrer(this);
    }

    /// <summary>
    /// Notification deferrer helper class
    /// </summary>
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class NotificationDeferrer : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        private readonly AdvancedCollectionView _acvs;
        private readonly object _currentItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDeferrer"/> class.
        /// </summary>
        /// <param name="acvs">Source ACVS</param>
        public NotificationDeferrer(AdvancedCollectionView acvs)
        {
            _acvs = acvs;
            _currentItem = _acvs.CurrentItem;
            _acvs._deferCounter++;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            _acvs.MoveCurrentTo(_currentItem);
            _acvs._deferCounter--;
            _acvs.Refresh();
        }
    }
}
