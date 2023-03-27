namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// SToken 包装器
/// </summary>
internal sealed class STokenWrapper
{
    public STokenWrapper(string stoken, string uid)
    {
        SToken = stoken;
        Uid = uid;
    }

    /// <summary>
    /// SToken
    /// </summary>
    [JsonPropertyName("stoken")]
    public string SToken { get; set; }

    /// <summary>
    /// Uid
    /// </summary>
    [JsonPropertyName("uid")]
    public string Uid { get; set; }
}