// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Web.Hutao;

internal sealed partial class IPInformation
{
    private const string Unknown = "Unknown";

    public static IPInformation Default { get; } = new()
    {
        Ip = Unknown,
        Division = Unknown,
    };

    [JsonPropertyName("ip")]
    public required string Ip { get; init; }

    [JsonPropertyName("division")]
    public required string Division { get; init; }

    [GeneratedRegex(@"(\d+)\.(\d+)\.\d+\.\d+")]
    private static partial Regex IpRegex { get; }

    public override string ToString()
    {
        if (Ip is Unknown && Division is Unknown)
        {
            return SH.WebHutaoServiceUnAvailable;
        }

        string maskedIp = IpRegex.Replace(Ip, "$1.$2.*.*");
        return SH.FormatViewPageSettingDeviceIpDescription(maskedIp, Division);
    }
}