// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Service.Navigation;

internal interface INavigationInitialization
{
    bool IsXamlElementAttached { get; }

    void AttachXamlElement(NavigationView navigationView, Frame frame);
}