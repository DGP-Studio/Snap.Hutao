// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

[DependencyProperty("ChineseSource", typeof(string))]
[DependencyProperty("OverseaSource", typeof(string))]
internal sealed partial class StaticJSBridgeUriSource : DependencyObject, IJSBridgeUriSource
{
    public MiHoYoJSBridgeFacade CreateJSBridge(IServiceProvider serviceProvider, CoreWebView2 coreWebView2, UserAndUid userAndUid)
    {
        return ActivatorUtilities.CreateInstance<MiHoYoJSBridgeFacade>(serviceProvider, coreWebView2, userAndUid);
    }

    public string GetSource(UserAndUid userAndUid)
    {
        return userAndUid.User.IsOversea ? OverseaSource : ChineseSource;
    }
}