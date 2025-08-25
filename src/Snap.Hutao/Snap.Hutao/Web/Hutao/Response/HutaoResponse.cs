// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hutao.Response;

internal sealed class HutaoResponse : Web.Response.Response, ILocalizableResponse, ICommonResponse<HutaoResponse>
{
    [JsonConstructor]
    public HutaoResponse(int returnCode, string message, string? localizationKey)
        : base(returnCode, message)
    {
        LocalizationKey = localizationKey;
    }

    [JsonPropertyName("l10nKey")]
    public string? LocalizationKey { get; set; }

    static HutaoResponse ICommonResponse<HutaoResponse>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message, default);
    }

    public override string ToString()
    {
        return SH.FormatWebResponse(ReturnCode, this.GetLocalizationMessageOrDefault());
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class HutaoResponse<TData> : Response<TData>, ILocalizableResponse, ICommonResponse<HutaoResponse<TData>>
{
    [JsonConstructor]
    public HutaoResponse(int returnCode, string message, TData? data, string? localizationKey)
        : base(returnCode, message, data)
    {
        LocalizationKey = localizationKey;
    }

    [JsonPropertyName("l10nKey")]
    public string? LocalizationKey { get; set; }

    static HutaoResponse<TData> ICommonResponse<HutaoResponse<TData>>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message, default, default);
    }

    public override string ToString()
    {
        return SH.FormatWebResponse(ReturnCode, this.GetLocalizationMessageOrDefault());
    }
}