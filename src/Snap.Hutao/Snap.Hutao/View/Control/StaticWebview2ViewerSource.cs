// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.View.Control;

[DependencyProperty("Source", typeof(string))]
internal sealed partial class StaticWebview2ViewerSource : DependencyObject, IWebViewerSource
{
    public string GetSource(UserAndUid userAndUid)
    {
        return Source;
    }
}