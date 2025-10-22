// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core.LifeCycle;

internal interface ICurrentXamlWindowReference<TWindow>
    where TWindow : Window
{
    TWindow? Window { get; set; }
}