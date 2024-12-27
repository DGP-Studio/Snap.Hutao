// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.Diagnostics;

namespace Snap.Hutao.Service.Hutao;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class HutaoUserOptions : ObservableObject
{
    private readonly IServiceProvider serviceProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private readonly AsyncKeyedLock<string> operationLock = new();
    private readonly AsyncManualResetEvent loginEvent = new();
    private readonly AsyncManualResetEvent infoEvent = new();

    private AuthTokenExpiration authTokenExpiration;

    [ObservableProperty]
    public partial bool IsLoggedIn { get; set; }

    [ObservableProperty]
    public partial string? UserName { get; set; } = SH.ViewServiceHutaoUserLoginOrRegisterHint;

    [ObservableProperty]
    public partial bool IsHutaoCloudAllowed { get; set; }

    [ObservableProperty]
    public partial string? CloudExpireAt { get; set; }

    [ObservableProperty]
    public partial bool IsHutaoCdnAllowed { get; set; }

    [ObservableProperty]
    public partial string? CdnExpireAt { get; set; }

    [ObservableProperty]
    public partial bool IsDeveloper { get; set; }

    [ObservableProperty]
    public partial bool IsMaintainer { get; set; }

    public async ValueTask<string?> GetActualUserNameAsync()
    {
        await infoEvent.WaitAsync().ConfigureAwait(false);
        return IsLoggedIn ? UserName : default;
    }

    public async ValueTask<bool> GetIsHutaoCdnAllowedAsync()
    {
        await infoEvent.WaitAsync().ConfigureAwait(false);
        return IsHutaoCdnAllowed;
    }

    public async ValueTask<string?> GetAuthTokenAsync()
    {
        using (await operationLock.LockAsync(nameof(GetAuthTokenAsync)).ConfigureAwait(false))
        {
            await loginEvent.WaitAsync().ConfigureAwait(false);

            if (!IsLoggedIn)
            {
                return default;
            }

            if (authTokenExpiration.ExpireAt < DateTimeOffset.UtcNow)
            {
                // Re-initialize to refresh the token
                await InitializeAsync().ConfigureAwait(false);
            }

            if (!IsLoggedIn)
            {
                return default;
            }

            return authTokenExpiration.Token;
        }
    }

    public async ValueTask InitializeAsync(CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(InitializeAsync)).ConfigureAwait(false))
        {
            string username = LocalSetting.Get(SettingKeys.PassportUserName, string.Empty);
            string password = LocalSetting.Get(SettingKeys.PassportPassword, string.Empty);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                loginEvent.Set();
                infoEvent.Set();
                return;
            }

            loginEvent.Reset();
            infoEvent.Reset();
            await LoginAsync(username, password, true, token).ConfigureAwait(false);
        }
    }

    [SuppressMessage("", "SH003")]
    public Task WaitUserInfoInitializationAsync()
    {
        return infoEvent.WaitAsync();
    }

    public async ValueTask LoginAsync(string username, string password, bool resuming = false, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(LoginAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                HutaoResponse<string> response = await hutaoPassportClient.LoginAsync(username, password, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out string? authToken))
                {
                    await taskContext.SwitchToMainThreadAsync();
                    UserName = SH.ViewServiceHutaoUserLoginFailHint;
                    loginEvent.Set();
                    infoEvent.Set();
                    return;
                }

                if (!resuming)
                {
                    infoBarService.Information(response.GetLocalizationMessageOrMessage());
                }

                await AcceptAuthTokenAsync(username, password, authToken, token).ConfigureAwait(false);
            }
        }
    }

    public async ValueTask RegisterAsync(string username, string password, string verifyCode, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(RegisterAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                HutaoResponse<string> response = await hutaoPassportClient.RegisterAsync(username, password, verifyCode, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out string? authToken))
                {
                    await taskContext.SwitchToMainThreadAsync();
                    UserName = SH.ViewServiceHutaoUserLoginFailHint;
                    loginEvent.Set();
                    infoEvent.Set();
                    return;
                }

                infoBarService.Information(response.GetLocalizationMessageOrMessage());
                await AcceptAuthTokenAsync(username, password, authToken, token).ConfigureAwait(false);
            }
        }
    }

    public async ValueTask UnregisterAsync(string username, string password, string verifyCode, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(UnregisterAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                HutaoResponse response = await hutaoPassportClient.UnregisterAsync(username, password, verifyCode, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider))
                {
                    // Should not happen
                    Debugger.Break();
                }

                infoBarService.Information(response.GetLocalizationMessageOrMessage());

                await taskContext.SwitchToMainThreadAsync();
                await LogoutOrUnregisterAsync().ConfigureAwait(false);
            }
        }
    }

    public async ValueTask LogoutAsync()
    {
        using (await operationLock.LockAsync(nameof(LogoutAsync)).ConfigureAwait(false))
        {
            await LogoutOrUnregisterAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask ResetPasswordAsync(string username, string password, string verifyCode, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(ResetPasswordAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                HutaoResponse<string> response = await hutaoPassportClient.ResetPasswordAsync(username, password, verifyCode, token).ConfigureAwait(false);

                if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out string? authToken))
                {
                    infoBarService.Information(response.GetLocalizationMessageOrMessage());
                    await AcceptAuthTokenAsync(username, password, authToken, token).ConfigureAwait(false);
                }
            }
        }
    }

    private async ValueTask AcceptAuthTokenAsync(string username, string password, string authToken, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(AcceptAuthTokenAsync)).ConfigureAwait(false))
        {
            LocalSetting.Set(SettingKeys.PassportUserName, username);
            LocalSetting.Set(SettingKeys.PassportPassword, password);

            await taskContext.SwitchToMainThreadAsync();
            UserName = username;
            authTokenExpiration = new(authToken);
            IsLoggedIn = true;
            loginEvent.Set();

            await taskContext.SwitchToBackgroundAsync();
            HutaoPassportClient passportClient = serviceProvider.GetRequiredService<HutaoPassportClient>();
            Response<UserInfo> userInfoResponse = await passportClient.GetUserInfoAsync(token).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(userInfoResponse, serviceProvider, out UserInfo? userInfo))
            {
                await taskContext.SwitchToMainThreadAsync();
                IsDeveloper = userInfo.IsLicensedDeveloper;
                IsMaintainer = userInfo.IsMaintainer;

                IsHutaoCloudAllowed = IsDeveloper || userInfo.GachaLogExpireAt > DateTimeOffset.Now;
                CloudExpireAt = $"{userInfo.GachaLogExpireAt:yyyy.MM.dd HH:mm:ss}";
                IsHutaoCdnAllowed = IsDeveloper || userInfo.CdnExpireAt > DateTimeOffset.Now;
                CdnExpireAt = $"{userInfo.CdnExpireAt:yyyy.MM.dd HH:mm:ss}";
            }

            infoEvent.Set();
        }
    }

    private async ValueTask LogoutOrUnregisterAsync()
    {
        using (await operationLock.LockAsync(nameof(LogoutOrUnregisterAsync)).ConfigureAwait(false))
        {
            LocalSetting.Set(SettingKeys.PassportUserName, string.Empty);
            LocalSetting.Set(SettingKeys.PassportPassword, string.Empty);

            await taskContext.SwitchToMainThreadAsync();
            authTokenExpiration = default;
            UserName = default;
            IsLoggedIn = false;
            IsDeveloper = false;
            IsMaintainer = false;
            IsHutaoCloudAllowed = false;
            CloudExpireAt = default;
            IsHutaoCdnAllowed = false;
            CdnExpireAt = default;
        }
    }

    private readonly struct AuthTokenExpiration
    {
        public readonly string Token;
        public readonly DateTimeOffset ExpireAt;

        public AuthTokenExpiration(string token)
        {
            Token = token;
            ExpireAt = DateTimeOffset.Now + TimeSpan.FromHours(2);
        }
    }
}