// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

internal sealed class GachaLogFetchStatus
{
    public GachaLogFetchStatus(GachaType configType)
    {
        ConfigType = configType;
    }

    public bool AuthKeyTimeout { get; set; }

    public GachaType ConfigType { get; }

    public List<Item> Items { get; } = new(20);

    public string Header
    {
        get
        {
            return AuthKeyTimeout
                ? SH.ViewDialogGachaLogRefreshProgressAuthkeyTimeout
                : SH.FormatViewDialogGachaLogRefreshProgressDescription(ConfigType.GetLocalizedDescription());
        }
    }
}