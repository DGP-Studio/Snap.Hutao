// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Service.Navigation;

internal interface INavigationService
{
    NavigationResult Navigate(Type pageType, INavigationCompletionSource data, bool syncNavigationViewItem = false);

    NavigationResult Navigate<T>(INavigationCompletionSource data, bool syncNavigationViewItem = false)
        where T : Page;

    ValueTask<NavigationResult> NavigateAsync<TPage>(INavigationCompletionSource data, bool syncNavigationViewItem = false)
        where TPage : Page;
}