// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao;

internal sealed partial class IdentifyMonitorWindow : Window
{
    public IdentifyMonitorWindow(string monitor)
    {
        InitializeComponent();

        Monitor = monitor;
    }

    public string Monitor { get; private set; }
}
