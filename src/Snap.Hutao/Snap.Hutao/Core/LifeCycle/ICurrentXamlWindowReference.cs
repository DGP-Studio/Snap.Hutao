// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core.LifeCycle;

[Obsolete]
internal interface ICurrentXamlWindowReference
{
    Window? Window { get; set; }
}

internal interface ICurrentXamlWindowReference<TWindow>
    where TWindow : Window
{
    [DisallowNull]
    TWindow? Window { get; set; }
}