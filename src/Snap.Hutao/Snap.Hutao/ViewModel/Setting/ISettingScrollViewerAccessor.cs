// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.UI.Xaml;

namespace Snap.Hutao.ViewModel.Setting;

internal interface ISettingScrollViewerAccessor : IXamlElementAccessor
{
    ScrollViewer ScrollViewer { get; }

    Border GachaLogBorder { get; }
}