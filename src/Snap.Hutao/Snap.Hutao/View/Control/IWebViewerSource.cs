// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.View.Control;

internal interface IWebViewerSource
{
    string GetSource(UserAndUid userAndUid);
}