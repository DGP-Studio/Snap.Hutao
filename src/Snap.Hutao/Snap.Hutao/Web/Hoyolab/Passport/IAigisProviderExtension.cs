// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Behavior.Action;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal static class IAigisProviderExtension
{
    public static async ValueTask<bool> TryResolveAigisAsync(this IAigisProvider provider, string? rawSession, bool isOversea, ITaskContext taskContext)
    {
        if (string.IsNullOrEmpty(rawSession))
        {
            return false;
        }

        AigisObject? session = JsonSerializer.Deserialize<AigisObject>(rawSession);
        ArgumentNullException.ThrowIfNull(session);
        AigisData? sessionData = JsonSerializer.Deserialize<AigisData>(session.Data);
        ArgumentNullException.ThrowIfNull(sessionData);

        await taskContext.SwitchToMainThreadAsync();
        GeetestWebView2ContentProvider contentProvider = new(sessionData.GT, sessionData.Challenge, isOversea);

        new ShowWebView2WindowAction
        {
            ContentProvider = contentProvider,
        }.ShowAt(provider.XamlRoot);

        await taskContext.SwitchToBackgroundAsync();
        string? result = await contentProvider.GetResultAsync().ConfigureAwait(false);

        if (string.IsNullOrEmpty(result))
        {
            // User closed the window without completing the verification
            return false;
        }

        provider.Aigis = $"{session.SessionId};{Convert.ToBase64String(Encoding.UTF8.GetBytes(result))}";
        return true;
    }

    private sealed class AigisObject
    {
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = default!;

        [JsonPropertyName("mmt_type")]
        public int MmtType { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; } = default!;
    }

    private sealed class AigisData
    {
        [JsonPropertyName("success")]
        public int Success { get; set; }

        [JsonPropertyName("gt")]
        public string GT { get; set; } = default!;

        [JsonPropertyName("challenge")]
        public string Challenge { get; set; } = default!;

        [JsonPropertyName("new_captcha")]
        public int NewCaptcha { get; set; }
    }
}