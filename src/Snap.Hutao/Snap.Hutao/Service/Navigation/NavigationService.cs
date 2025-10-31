// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation.Message;

namespace Snap.Hutao.Service.Navigation;

[Service(ServiceLifetime.Singleton, typeof(INavigationService))]
internal sealed partial class NavigationService : INavigationService
{
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial NavigationService(IServiceProvider serviceProvider);

    public NavigationResult Navigate(Type pageType, INavigationCompletionSource data, bool syncNavigationViewItem = false)
    {
        NavigationNavigateMessage message = new()
        {
            PageType = pageType,
            Data = data,
            SyncNavigationViewItem = syncNavigationViewItem,
        };

        messenger.Send(message);
        return message.Result;
    }

    public NavigationResult Navigate<TPage>(INavigationCompletionSource data, bool syncNavigationViewItem = false)
        where TPage : Page
    {
        return Navigate(typeof(TPage), data, syncNavigationViewItem);
    }

    public async ValueTask<NavigationResult> NavigateAsync<TPage>(INavigationCompletionSource data, bool syncNavigationViewItem = false)
        where TPage : Page
    {
        await taskContext.SwitchToMainThreadAsync();
        NavigationResult result = Navigate<TPage>(data, syncNavigationViewItem);

        if (result is NavigationResult.Succeed)
        {
            try
            {
                await taskContext.SwitchToBackgroundAsync();
                await data.WaitForCompletionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return NavigationResult.Failed;
            }
        }

        return result;
    }
}