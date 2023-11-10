// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.View.Control;

[DependencyProperty("ChineseSource", typeof(string))]
[DependencyProperty("OverseaSource", typeof(string))]
internal sealed partial class StaticWebview2ViewerSource : DependencyObject, IWebViewerSource
{
    public MiHoYoJSBridge CreateJSBridge(IServiceProvider serviceProvider, CoreWebView2 coreWebView2, UserAndUid userAndUid)
    {
        return serviceProvider.CreateInstance<MiHoYoJSBridge>(coreWebView2, userAndUid);
    }

    public string GetSource(UserAndUid userAndUid)
    {
        return userAndUid.User.IsOversea ? OverseaSource : ChineseSource;
    }
}