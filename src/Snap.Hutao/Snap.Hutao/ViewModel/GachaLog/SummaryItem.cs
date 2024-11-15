// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;

namespace Snap.Hutao.ViewModel.GachaLog;

internal sealed class SummaryItem : Item
{
    public bool IsUp { get; set; }

    public bool IsGuarantee { get; set; }

    public int GuaranteeOrangeThreshold { get; set; }

    public int LastPull { get; set; }

    public string TimeFormatted
    {
        get => $"{Time.ToLocalTime():yyy.MM.dd HH:mm:ss}";
    }

    public Windows.UI.Color Color { get; set; }

    internal DateTimeOffset Time { get; set; }
}