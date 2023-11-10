// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Snap.Hutao.View.Control;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Bridge;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Request.QueryString;

namespace Snap.Hutao.ViewModel.DailyNote;

internal sealed class DailyNoteWebViewerSource : IWebViewerSource
{
    public MiHoYoJSBridge CreateJSBridge(IServiceProvider serviceProvider, CoreWebView2 coreWebView2, UserAndUid userAndUid)
    {
        return serviceProvider.CreateInstance<MiHoYoJSBridge>(coreWebView2, userAndUid);
    }

    public string GetSource(UserAndUid userAndUid)
    {
        QueryString query = userAndUid.Uid.ToQueryString();
        return $"https://webstatic.mihoyo.com/app/community-game-records/index.html?bbs_presentation_style=fullscreen#/ys/daily/?{query}";
    }
}