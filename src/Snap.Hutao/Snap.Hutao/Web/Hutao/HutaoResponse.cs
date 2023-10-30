// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Web.Hutao;

internal sealed class HutaoResponse : Response.Response, ILocalizableResponse
{
    [JsonConstructor]
    public HutaoResponse(int returnCode, string message, string? localizationKey)
        : base(returnCode, message)
    {
        LocalizationKey = localizationKey;
    }

    [JsonPropertyName("l10nKey")]
    public string? LocalizationKey { get; set; }

    public static HutaoResponse DefaultIfNull(HutaoResponse? response, [CallerMemberName] string callerName = default!)
    {
        // 0x26F19335 is a magic number that hashed from "Snap.Hutao"
        response ??= new(InternalFailure, SH.WebResponseRequestExceptionFormat.Format(callerName, null), default);
        return response;
    }

    public static HutaoResponse<TData> DefaultIfNull<TData>(HutaoResponse<TData>? response, [CallerMemberName] string callerName = default!)
    {
        // 0x26F19335 is a magic number that hashed from "Snap.Hutao"
        response ??= new(InternalFailure, SH.WebResponseRequestExceptionFormat.Format(callerName, typeof(TData).Name), default, default);
        return response ?? new(InternalFailure, SH.WebResponseRequestExceptionFormat.Format(callerName, typeof(TData).Name), default, default);
    }

    public override string ToString()
    {
        return SH.WebResponseFormat.Format(ReturnCode, this.GetLocalizationMessageOrDefault());
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class HutaoResponse<TData> : Response.Response<TData>, ILocalizableResponse
{
    [JsonConstructor]
    public HutaoResponse(int returnCode, string message, TData? data, string? localizationKey)
        : base(returnCode, message, data)
    {
        LocalizationKey = localizationKey;
    }

    [JsonPropertyName("l10nKey")]
    public string? LocalizationKey { get; set; }

    public override string ToString()
    {
        return SH.WebResponseFormat.Format(ReturnCode, this.GetLocalizationMessageOrDefault());
    }
}