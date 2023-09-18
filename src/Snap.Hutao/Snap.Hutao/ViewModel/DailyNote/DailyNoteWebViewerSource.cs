// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.View.Control;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Request.QueryString;

namespace Snap.Hutao.ViewModel.DailyNote;

internal sealed class DailyNoteWebViewerSource : IWebViewerSource
{
    public string GetSource(UserAndUid userAndUid)
    {
        QueryString query = userAndUid.Uid.ToQueryString();
        return $"https://webstatic.mihoyo.com/app/community-game-records/index.html?bbs_presentation_style=fullscreen#/ys/daily/?{query}";
    }
}