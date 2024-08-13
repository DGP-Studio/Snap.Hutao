// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Service.User;

internal sealed class InputCookie : IDeconstruct<Cookie, bool, string?>
{
    private InputCookie(Cookie cookie, bool isOversea)
    {
        Cookie = cookie;
        IsOversea = isOversea;
        cookie.TryGetDeviceFp(out string? deviceFp);
        DeviceFp = deviceFp;
    }

    private InputCookie(Cookie cookie, bool isOversea, string? deviceFp)
    {
        Cookie = cookie;
        IsOversea = isOversea;
        DeviceFp = deviceFp;
    }

    public Cookie Cookie { get; }

    public bool IsOversea { get; }

    public string? DeviceFp { get; }

    public static InputCookie Create(Cookie cookie, bool isOversea, string? deviceFp)
    {
        return new InputCookie(cookie, isOversea, deviceFp);
    }

    public static InputCookie CreateForDeviceFpInference(Cookie cookie, bool isOversea)
    {
        return new InputCookie(cookie, isOversea);
    }

    public void Deconstruct(out Cookie cookie, out bool isOversea, out string? deviceFp)
    {
        cookie = Cookie;
        isOversea = IsOversea;
        deviceFp = DeviceFp;
    }
}