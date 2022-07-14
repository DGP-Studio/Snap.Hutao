// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Navigation;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 角色资料页
/// </summary>
public sealed partial class WikiAvatarPage : Microsoft.UI.Xaml.Controls.Page
{
    /// <summary>
    /// 构造一个新的角色资料页
    /// </summary>
    public WikiAvatarPage()
    {
        DataContext = Ioc.Default.GetRequiredService<WikiAvatarViewModel>();
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is INavigationData extra)
        {
            extra.NotifyNavigationCompleted();
        }
    }
}
