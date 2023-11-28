namespace Snap.Hutao.Web.Hutao;

internal sealed class IPInfo
{
    public IPInfo()
    {
    }

    [JsonConstructor]
    public IPInfo(string ip, string division)
    {
        Ip = ip;
        Division = division;
    }

    public string Ip { get; set; } = "Unknown";

    public string Division { get; set; } = "Unknown";

    public override string ToString()
    {
        return SH.FormatViewPageSettingDeviceIpDescription(Ip, Division);
    }
}
