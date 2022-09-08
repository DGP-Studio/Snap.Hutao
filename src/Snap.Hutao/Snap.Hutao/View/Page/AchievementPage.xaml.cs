// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Navigation;
using Snap.Hutao.Control.Cancellable;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 成就页面
/// </summary>
public sealed partial class AchievementPage : CancellablePage
{
    /// <summary>
    /// 构造一个新的成就页面
    /// </summary>
    public AchievementPage()
    {
        InitializeWith<AchievementViewModel>();
        InitializeComponent();
    }

    /// <inheritdoc/>
    [SuppressMessage("", "VSTHRD100")]
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is INavigationData extra)
        {
            if (extra.Data != null)
            {
                await ((INavigationRecipient)DataContext).ReceiveAsync(extra).ConfigureAwait(false);
            }

            extra.NotifyNavigationCompleted();
        }
    }
}
