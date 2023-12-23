// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Bridge.Model;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Web.Response;

internal class Response : ICommonResponse<Response>
{
    public const int InternalFailure = 0x26F19335;

    [JsonConstructor]
    public Response(int returnCode, string message)
    {
        ReturnCode = returnCode;
        Message = message;
#if DEBUG
        Ioc.Default.GetRequiredService<ILogger<Response>>().LogInformation("Response [{resp}]", ToString());
#endif
    }

    [JsonPropertyName("retcode")]
    public int ReturnCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

    public static implicit operator ValueResult<bool, string>(Response response)
    {
        return new(response.ReturnCode == 0, response.Message);
    }

    static Response ICommonResponse<Response>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message);
    }

    public static TResponse DefaultIfNull<TResponse>(TResponse? response, [CallerMemberName] string callerName = default!)
        where TResponse : ICommonResponse<TResponse>
    {
        string message = SH.FormatWebResponseRequestExceptionFormat(callerName, TypeNameHelper.GetTypeDisplayName(typeof(TResponse)));
        response ??= TResponse.CreateDefault(InternalFailure, message);

        if (((KnownReturnCode)response.ReturnCode) is KnownReturnCode.PleaseLogin or KnownReturnCode.RET_TOKEN_INVALID)
        {
            response.Message = SH.FormatWebResponseRefreshCookieHintFormat(response.Message);
        }

        return response;
    }

    public static Response<TData> CloneReturnCodeAndMessage<TData, TOther>(Response<TOther> response, [CallerMemberName] string callerName = default!)
    {
        return new(response.ReturnCode, response.Message, default);
    }

    public virtual bool IsOk(bool showInfoBar = true, IServiceProvider? serviceProvider = null)
    {
        if (ReturnCode == 0)
        {
            return true;
        }
        else
        {
            if (showInfoBar)
            {
                serviceProvider ??= Ioc.Default;
                serviceProvider.GetRequiredService<IInfoBarService>().Error(ToString());
            }

            return false;
        }
    }

    public override string ToString()
    {
        return SH.FormatWebResponseFormat(ReturnCode, Message);
    }
}

[SuppressMessage("", "SA1402")]
internal class Response<TData> : Response, ICommonResponse<Response<TData>>, IJsBridgeResult
{
    [JsonConstructor]
    public Response(int returnCode, string message, TData? data)
        : base(returnCode, message)
    {
        Data = data;
    }

    [JsonPropertyName("data")]
    public TData? Data { get; set; }

    static Response<TData> ICommonResponse<Response<TData>>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message, default);
    }

    [MemberNotNullWhen(true, nameof(Data))]
    public override bool IsOk(bool showInfoBar = true, IServiceProvider? serviceProvider = null)
    {
        if (ReturnCode == 0)
        {
            ArgumentNullException.ThrowIfNull(Data);
            return true;
        }
        else
        {
            if (showInfoBar)
            {
                serviceProvider ??= Ioc.Default;
                serviceProvider.GetRequiredService<IInfoBarService>().Error(ToString());
            }

            return false;
        }
    }
}