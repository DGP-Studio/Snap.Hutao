#pragma warning disable
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.Interop.WinHttp;

namespace Snap.Hutao.Core.IO.Http.DynamicProxy;

/// <summary>
/// Copied from https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/Common/src/System/Net/Http/WinInetProxyHelper.cs
/// </summary>
internal sealed class WinInetProxyHelper
{
    private const int RecentAutoDetectionInterval = 120_000; // 2 minutes in milliseconds.
    private readonly string? _autoConfigUrl, _proxy, _proxyBypass;
    private readonly bool _autoDetect;
    private readonly bool _useProxy;
    private bool _autoDetectionFailed;
    private int _lastTimeAutoDetectionFailed; // Environment.TickCount units (milliseconds).

    public WinInetProxyHelper()
    {
        WINHTTP_CURRENT_USER_IE_PROXY_CONFIG proxyConfig = default;

        try
        {
            if (WinHttpGetIEProxyConfigForCurrentUser(out proxyConfig))
            {
                _autoConfigUrl = Marshal.PtrToStringUni(proxyConfig.AutoConfigUrl)!;
                _autoDetect = proxyConfig.AutoDetect != 0;
                _proxy = Marshal.PtrToStringUni(proxyConfig.Proxy)!;
                _proxyBypass = Marshal.PtrToStringUni(proxyConfig.ProxyBypass)!;

                _useProxy = true;
            }
            else
            {
                // We match behavior of WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY and ignore errors.
                int lastError = Marshal.GetLastWin32Error();
            }
        }
        finally
        {
            // FreeHGlobal already checks for null pointer before freeing the memory.
            Marshal.FreeHGlobal(proxyConfig.AutoConfigUrl);
            Marshal.FreeHGlobal(proxyConfig.Proxy);
            Marshal.FreeHGlobal(proxyConfig.ProxyBypass);
        }
    }

    public string? AutoConfigUrl => _autoConfigUrl;

    public bool AutoDetect => _autoDetect;

    public bool AutoSettingsUsed => AutoDetect || !string.IsNullOrEmpty(AutoConfigUrl);

    public bool ManualSettingsUsed => !string.IsNullOrEmpty(Proxy);

    public bool ManualSettingsOnly => !AutoSettingsUsed && ManualSettingsUsed;

    public string? Proxy => _proxy;

    public string? ProxyBypass => _proxyBypass;

    public bool RecentAutoDetectionFailure =>
        _autoDetectionFailed &&
        Environment.TickCount - _lastTimeAutoDetectionFailed <= RecentAutoDetectionInterval;

    public bool GetProxyForUrl(
        SafeWinHttpHandle? sessionHandle,
        Uri uri,
        out WINHTTP_PROXY_INFO proxyInfo)
    {
        proxyInfo.AccessType = WINHTTP_ACCESS_TYPE_NO_PROXY;
        proxyInfo.Proxy = IntPtr.Zero;
        proxyInfo.ProxyBypass = IntPtr.Zero;

        if (!_useProxy)
        {
            return false;
        }

        bool useProxy = false;

        WINHTTP_AUTOPROXY_OPTIONS autoProxyOptions;
        autoProxyOptions.AutoConfigUrl = AutoConfigUrl;
        autoProxyOptions.AutoDetectFlags = AutoDetect ?
            (WINHTTP_AUTO_DETECT_TYPE_DHCP | WINHTTP_AUTO_DETECT_TYPE_DNS_A) : 0;
        autoProxyOptions.AutoLoginIfChallenged = false;
        autoProxyOptions.Flags =
            (AutoDetect ? WINHTTP_AUTOPROXY_AUTO_DETECT : 0) |
            (!string.IsNullOrEmpty(AutoConfigUrl) ? WINHTTP_AUTOPROXY_CONFIG_URL : 0);
        autoProxyOptions.Reserved1 = IntPtr.Zero;
        autoProxyOptions.Reserved2 = 0;

        // AutoProxy Cache.
        // https://docs.microsoft.com/en-us/windows/desktop/WinHttp/autoproxy-cache
        // If the out-of-process service is active when WinHttpGetProxyForUrl is called, the cached autoproxy
        // URL and script are available to the whole computer. However, if the out-of-process service is used,
        // and the fAutoLogonIfChallenged flag in the pAutoProxyOptions structure is true, then the autoproxy
        // URL and script are not cached. Therefore, calling WinHttpGetProxyForUrl with the fAutoLogonIfChallenged
        // member set to TRUE results in additional overhead operations that may affect performance.
        // The following steps can be used to improve performance:
        // 1. Call WinHttpGetProxyForUrl with the fAutoLogonIfChallenged parameter set to false. The autoproxy
        //    URL and script are cached for future calls to WinHttpGetProxyForUrl.
        // 2. If Step 1 fails, with ERROR_WINHTTP_LOGIN_FAILURE, then call WinHttpGetProxyForUrl with the
        //    fAutoLogonIfChallenged member set to TRUE.
        //
        // We match behavior of WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY and ignore errors.
#pragma warning disable CA1845 // file is shared with a build that lacks string.Concat for spans
        // Underlying code does not understand WebSockets so we need to convert it to http or https.
        string destination = uri.AbsoluteUri;
        if (uri.Scheme == UriScheme.Wss)
        {
            destination = UriScheme.Https + destination.Substring(UriScheme.Wss.Length);
        }
        else if (uri.Scheme == UriScheme.Ws)
        {
            destination = UriScheme.Http + destination.Substring(UriScheme.Ws.Length);
        }
#pragma warning restore CA1845

        var repeat = false;
        do
        {
            _autoDetectionFailed = false;
            if (WinHttpGetProxyForUrl(
                sessionHandle!,
                destination,
                ref autoProxyOptions,
                out proxyInfo))
            {
                useProxy = true;

                break;
            }
            else
            {
                var lastError = Marshal.GetLastWin32Error();

                if (lastError == ERROR_WINHTTP_LOGIN_FAILURE)
                {
                    if (repeat)
                    {
                        // We don't retry more than once.
                        break;
                    }
                    else
                    {
                        repeat = true;
                        autoProxyOptions.AutoLoginIfChallenged = true;
                    }
                }
                else
                {
                    if (lastError == ERROR_WINHTTP_AUTODETECTION_FAILED)
                    {
                        _autoDetectionFailed = true;
                        _lastTimeAutoDetectionFailed = Environment.TickCount;
                    }

                    break;
                }
            }
        } while (repeat);

        // Fall back to manual settings if available.
        if (!useProxy && !string.IsNullOrEmpty(Proxy))
        {
            proxyInfo.AccessType = WINHTTP_ACCESS_TYPE_NAMED_PROXY;
            proxyInfo.Proxy = Marshal.StringToHGlobalUni(Proxy);
            proxyInfo.ProxyBypass = string.IsNullOrEmpty(ProxyBypass) ?
                IntPtr.Zero : Marshal.StringToHGlobalUni(ProxyBypass);

            useProxy = true;
        }

        return useProxy;
    }
}
#pragma warning restore