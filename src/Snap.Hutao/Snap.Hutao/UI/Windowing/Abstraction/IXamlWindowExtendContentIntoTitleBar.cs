// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Windowing.Abstraction;

internal interface IXamlWindowExtendContentIntoTitleBar
{
    FrameworkElement TitleBarCaptionAccess { get; }

    IEnumerable<FrameworkElement> TitleBarPassthrough { get; }
}