// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class IPInformation
{
    private const string Unknown = "Unknown";

    public static IPInformation Default { get; } = new()
    {
        Ip = Unknown,
        Division = Unknown,
    };

    [JsonPropertyName("ip")]
    public string Ip { get; set; } = default!;

    [JsonPropertyName("division")]
    public string Division { get; set; } = default!;

    public override string ToString()
    {
        if (Ip is Unknown && Division is Unknown)
        {
            return SH.WebHutaoServiceUnAvailable;
        }

        return SH.FormatViewPageSettingDeviceIpDescription(Ip, Division);
    }
}