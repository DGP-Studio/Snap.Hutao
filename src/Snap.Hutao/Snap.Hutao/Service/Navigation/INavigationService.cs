// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Service.Navigation;

internal interface INavigationService : INavigationCurrent
{
    NavigationResult Navigate(Type pageType, INavigationAwaiter data, bool isSyncTabRequested = false);

    NavigationResult Navigate<T>(INavigationAwaiter data, bool isSyncTabRequested = false)
        where T : Page;

    ValueTask<NavigationResult> NavigateAsync<TPage>(INavigationAwaiter data, bool syncNavigationViewItem = false)
        where TPage : Page;

    void GoBack();
}