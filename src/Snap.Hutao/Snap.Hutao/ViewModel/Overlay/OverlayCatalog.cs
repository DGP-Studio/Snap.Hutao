// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.UI.Input.HotKey;

namespace Snap.Hutao.ViewModel.Overlay;

internal sealed class OverlayCatalog
{
    public OverlayCatalog(string id, string icon, string name)
    {
        Id = id;
        Icon = icon;
        Name = name;
    }

    public string Id { get; }

    public string Icon { get; }

    public string Name { get; }
}