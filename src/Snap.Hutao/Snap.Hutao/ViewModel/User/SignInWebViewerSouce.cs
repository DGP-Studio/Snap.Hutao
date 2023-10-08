// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.View.Control;
using Snap.Hutao.Web.Bridge;

namespace Snap.Hutao.ViewModel.User;

internal sealed class SignInWebViewerSouce : DependencyObject, IWebViewerSource
{
    public MiHoYoJSInterface CreateJsInterface(IServiceProvider serviceProvider, CoreWebView2 coreWebView2, UserAndUid userAndUid)
    {
        return userAndUid.User.IsOversea
            ? serviceProvider.CreateInstance<SignInJSInterfaceOversea>(coreWebView2, userAndUid)
            : serviceProvider.CreateInstance<SignInJSInterface>(coreWebView2, userAndUid);
    }

    public string GetSource(UserAndUid userAndUid)
    {
        return userAndUid.User.IsOversea
            ? "https://act.hoyolab.com/ys/event/signin-sea-v3/index.html?act_id=e202102251931481"
            : "https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?act_id=e202009291139501";
    }
}