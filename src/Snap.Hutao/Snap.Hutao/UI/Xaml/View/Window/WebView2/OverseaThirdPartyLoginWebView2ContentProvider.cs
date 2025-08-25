// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using Snap.Hutao.Core;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.WebView2;
using System.Collections.Frozen;
using System.Text;
using Windows.Graphics;

namespace Snap.Hutao.UI.Xaml.View.Window.WebView2;

internal sealed class OverseaThirdPartyLoginWebView2ContentProvider : DependencyObject, IWebView2ContentProvider
{
    private const string BaseUrl = "https://account.hoyoverse.com/single-page/third-party-oauth.html";

    private static readonly FrozenDictionary<OverseaThirdPartyKind, string> ThirdPartyToType = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(OverseaThirdPartyKind.Google, "gl"),
        KeyValuePair.Create(OverseaThirdPartyKind.Apple, "ap"),
        KeyValuePair.Create(OverseaThirdPartyKind.Facebook, "fb"),
        KeyValuePair.Create(OverseaThirdPartyKind.Twitter, "tw"),
    ]);

    private static readonly FrozenDictionary<OverseaThirdPartyKind, string> ThirdPartyToClientId = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(OverseaThirdPartyKind.Google, "332303543001-mt3n63m59a8o33vs496a55ct6l42vipc.apps.googleusercontent.com"),
        KeyValuePair.Create(OverseaThirdPartyKind.Apple, "com.hoyoverse.platoversealogin"),
        KeyValuePair.Create(OverseaThirdPartyKind.Facebook, "2099441543493930"),
        KeyValuePair.Create(OverseaThirdPartyKind.Twitter, "R1liQ2o1TE8xWW43MUJaRFZzenE6MTpjaQ"),
    ]);

    private readonly OverseaThirdPartyKind kind;
    private readonly string targetUrl;
    private readonly TaskCompletionSource resultTcs = new();

    private ThirdPartyToken? result;

    public OverseaThirdPartyLoginWebView2ContentProvider(OverseaThirdPartyKind kind, string languageCode)
    {
        this.kind = kind;
        targetUrl = GetThirdLoginUrl(kind, languageCode);
    }

    public ElementTheme ActualTheme { get; set; }

    public CoreWebView2? CoreWebView2 { get; set; }

    public Action? CloseWindowAction { get; set; }

    public ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(CoreWebView2);

        CoreWebView2.DisableAutoCompletion();

        CoreWebView2.Navigate(targetUrl);
        CoreWebView2.NavigationStarting += OnNavigationStarting;

        return ValueTask.CompletedTask;
    }

    public RectInt32 InitializePosition(RectInt32 parentRect, double parentDpi)
    {
        return WebView2WindowPosition.Padding(parentRect, 48);
    }

    public void Unload()
    {
        if (CoreWebView2 is not null)
        {
            CoreWebView2.NavigationStarting -= OnNavigationStarting;
        }

        resultTcs.TrySetResult();
    }

    public async ValueTask<ThirdPartyToken?> GetResultAsync()
    {
        await resultTcs.Task.ConfigureAwait(false);
        return result;
    }

    private static string GetThirdLoginUrl(OverseaThirdPartyKind kind, string languageCode)
    {
#pragma warning disable CA1308
        StringBuilder baseQuery = new($"?client_id={ThirdPartyToClientId[kind]}&route={kind.ToString().ToLowerInvariant()}&callback_method=deeplink&message_id={Guid.NewGuid()}&lang={languageCode}&scheme=about%3Ablank");
#pragma warning restore CA1308
        switch (kind)
        {
            case OverseaThirdPartyKind.Google:
                baseQuery.Append("&scope=email profile openid");
                baseQuery.Append("&response_type=id_token token");
                break;
            case OverseaThirdPartyKind.Apple:
                baseQuery.Append("&response_mode=fragment");
                baseQuery.Append("&response_id=code id_token");
                break;
            case OverseaThirdPartyKind.Facebook:
                baseQuery.Append("&scope=email");
                baseQuery.Append("&response_type=token");
                break;
            case OverseaThirdPartyKind.Twitter:
                baseQuery.Append("&scope=users.read tweet.read");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }

        return $"{BaseUrl}{baseQuery}";
    }

    private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (e.Uri.StartsWith("about:blank", StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;
            ReadOnlySpan<char> uriSpan = e.Uri.AsSpan()[18..];
            uriSpan.TrySplitIntoTwo('&', out ReadOnlySpan<char> tokenSpan, out _);
            result = new(ThirdPartyToType[kind], Uri.UnescapeDataString(tokenSpan));
            CloseWindowAction?.Invoke();
        }
    }
}