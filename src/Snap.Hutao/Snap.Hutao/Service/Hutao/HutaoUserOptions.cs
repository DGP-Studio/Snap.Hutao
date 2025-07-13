// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Redeem;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;

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

    [SuppressMessage("", "SA1500")]
    [SuppressMessage("", "SA1503")]
    [SuppressMessage("", "SA1513")]
    public string? UserName
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                SentrySdk.ConfigureScope(static (scope, userName) =>
                {
                    scope.User.Email = string.IsNullOrEmpty(userName) || !userName.IsEmail() ? default : userName;
                }, value);
            }
        }
    } = SH.ViewServiceHutaoUserLoginOrRegisterHint;

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

    public async ValueTask<string?> GetAuthTokenAsync(CancellationToken token = default)
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
                await InitializeAsync(token).ConfigureAwait(false);
            }

            if (!IsLoggedIn)
            {
                return default;
            }

            return authTokenExpiration.Token;
        }
    }

    public async ValueTask<string?> DangerousGetAuthTokenAsync(CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(DangerousGetAuthTokenAsync)).ConfigureAwait(false))
        {
            await loginEvent.WaitAsync().ConfigureAwait(false);

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
            string refreshToken = LocalSetting.Get(SettingKeys.PassportRefreshToken, string.Empty);

            if (string.IsNullOrEmpty(username))
            {
                loginEvent.Set();
                infoEvent.Set();
                return;
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                loginEvent.Reset();
                infoEvent.Reset();
                await RefreshTokenAsync(username, refreshToken, token).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(password))
            {
                loginEvent.Reset();
                infoEvent.Reset();
                await LoginAsync(username, password, true, token).ConfigureAwait(false);
            }
            else
            {
                loginEvent.Set();
                infoEvent.Set();
                return;
            }
        }
    }

    [SuppressMessage("", "SH003")]
    public Task WaitUserInfoInitializationAsync()
    {
        return infoEvent.WaitAsync();
    }

    public async ValueTask RefreshUserInfoAsync(CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(RefreshUserInfoAsync)).ConfigureAwait(false))
        {
            // Wait previous Info Event
            await infoEvent.WaitAsync().ConfigureAwait(false);

            if (!IsLoggedIn)
            {
                return;
            }

            infoEvent.Reset();

            if (await GetAuthTokenAsync(token).ConfigureAwait(false) is not null)
            {
                await PrivateRefreshUserInfoAsync(token).ConfigureAwait(false);
            }
        }
    }

    public async ValueTask UseRedeemCodeAsync(string code, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(UseRedeemCodeAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoRedeemCodeClient hutaoRedeemCodeClient = scope.ServiceProvider.GetRequiredService<HutaoRedeemCodeClient>();
                HutaoResponse<RedeemUseResult> response = await hutaoRedeemCodeClient.UseRedeemCodeAsync(new(code), token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider))
                {
                    return;
                }

                infoBarService.Information(response.GetLocalizationMessageOrMessage());
                await RefreshUserInfoAsync(token).ConfigureAwait(false);
            }
        }
    }

    public async ValueTask LoginAsync(string username, string password, bool resuming = false, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(LoginAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                HutaoResponse<TokenSet> response = await hutaoPassportClient.LoginAsync(username, password, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out TokenSet? tokenSet))
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

                await AcceptAuthTokenAsync(username, tokenSet, token).ConfigureAwait(false);
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
                HutaoResponse<TokenSet> response = await hutaoPassportClient.RegisterAsync(username, password, verifyCode, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out TokenSet? tokenSet))
                {
                    await taskContext.SwitchToMainThreadAsync();
                    UserName = SH.ViewServiceHutaoUserLoginFailHint;
                    loginEvent.Set();
                    infoEvent.Set();
                    return;
                }

                infoBarService.Information(response.GetLocalizationMessageOrMessage());
                await AcceptAuthTokenAsync(username, tokenSet, token).ConfigureAwait(false);
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
                await hutaoPassportClient.RevokeAllTokensAsync(token).ConfigureAwait(false);
                HutaoResponse response = await hutaoPassportClient.UnregisterAsync(username, password, verifyCode, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider))
                {
                    return;
                }

                await LogoutOrUnregisterAsync().ConfigureAwait(false);
                infoBarService.Information(response.GetLocalizationMessageOrMessage());
            }
        }
    }

    public async ValueTask LogoutAsync()
    {
        using (await operationLock.LockAsync(nameof(LogoutAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                await hutaoPassportClient.RevokeTokenAsync(HutaoRuntime.DeviceId).ConfigureAwait(false);

                await LogoutOrUnregisterAsync().ConfigureAwait(false);
            }
        }
    }

    public async ValueTask ResetUserNameAsync(string username, string newUserName, string verifyCode, string newVerifyCode, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(ResetUserNameAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                HutaoResponse<TokenSet> response = await hutaoPassportClient.ResetUserNameAsync(username, newUserName, verifyCode, newVerifyCode, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out TokenSet? tokenSet))
                {
                    return;
                }

                infoBarService.Information(response.GetLocalizationMessageOrMessage());
                await AcceptAuthTokenAsync(newUserName, tokenSet, token).ConfigureAwait(false);
            }
        }
    }

    public async ValueTask ResetPasswordAsync(string username, string password, string verifyCode, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(ResetPasswordAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                HutaoResponse<TokenSet> response = await hutaoPassportClient.ResetPasswordAsync(username, password, verifyCode, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out TokenSet? tokenSet))
                {
                    return;
                }

                infoBarService.Information(response.GetLocalizationMessageOrMessage());
                await AcceptAuthTokenAsync(username, tokenSet, token).ConfigureAwait(false);
            }
        }
    }

    public async ValueTask RefreshTokenAsync(string username, string refreshToken, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(RefreshTokenAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HutaoPassportClient hutaoPassportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                HutaoResponse<TokenSet> response = await hutaoPassportClient.RefreshTokenAsync(refreshToken, token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out TokenSet? tokenSet))
                {
                    return;
                }

                await AcceptAuthTokenAsync(username, tokenSet, token).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask AcceptAuthTokenAsync(string username, TokenSet tokenSet, CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(AcceptAuthTokenAsync)).ConfigureAwait(false))
        {
            LocalSetting.Update(SettingKeys.PassportUserName, string.Empty, username);
            LocalSetting.Update(SettingKeys.PassportRefreshToken, string.Empty, tokenSet.RefreshToken);

            await taskContext.SwitchToMainThreadAsync();
            UserName = username;
            authTokenExpiration = new(tokenSet);
            IsLoggedIn = true;
            loginEvent.Set();

            await PrivateRefreshUserInfoAsync(token).ConfigureAwait(false);
        }
    }

    private async ValueTask LogoutOrUnregisterAsync()
    {
        using (await operationLock.LockAsync(nameof(LogoutOrUnregisterAsync)).ConfigureAwait(false))
        {
            LocalSetting.Set(SettingKeys.PassportUserName, string.Empty);
            LocalSetting.Set(SettingKeys.PassportRefreshToken, string.Empty);

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

    private async ValueTask PrivateRefreshUserInfoAsync(CancellationToken token = default)
    {
        using (await operationLock.LockAsync(nameof(PrivateRefreshUserInfoAsync)).ConfigureAwait(false))
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                await taskContext.SwitchToBackgroundAsync();
                HutaoPassportClient passportClient = scope.ServiceProvider.GetRequiredService<HutaoPassportClient>();
                Response<UserInfo> userInfoResponse = await passportClient.GetUserInfoAsync(token).ConfigureAwait(false);

                UserInfo? userInfo;
                try
                {
                    if (!ResponseValidator.TryValidate(userInfoResponse, scope.ServiceProvider, out userInfo))
                    {
                        infoEvent.Set();
                        return;
                    }
                }
                catch (ObjectDisposedException)
                {
                    return;
                }

                await taskContext.SwitchToMainThreadAsync();
                IsDeveloper = userInfo.IsLicensedDeveloper;
                IsMaintainer = userInfo.IsMaintainer;

                IsHutaoCloudAllowed = IsDeveloper || userInfo.GachaLogExpireAt > DateTimeOffset.Now;
                CloudExpireAt = userInfo.GachaLogExpireAt > DateTimeOffset.Now
                    ? $"{userInfo.GachaLogExpireAt:yyyy.MM.dd HH:mm:ss}"
                    : SH.ViewServiceHutaoUserCloudNotAllowedDescription;

                IsHutaoCdnAllowed = IsDeveloper || userInfo.CdnExpireAt > DateTimeOffset.Now;
                CdnExpireAt = userInfo.CdnExpireAt > DateTimeOffset.Now
                    ? $"{userInfo.CdnExpireAt:yyyy.MM.dd HH:mm:ss}"
                    : SH.ViewServiceHutaoUserCdnNotAllowedDescription;

                infoEvent.Set();
            }
        }
    }

    private readonly struct AuthTokenExpiration
    {
        public readonly string Token;
        public readonly DateTimeOffset ExpireAt;

        public AuthTokenExpiration(TokenSet tokenSet)
        {
            Token = tokenSet.AccessToken;
            ExpireAt = DateTimeOffset.Now + TimeSpan.FromSeconds(tokenSet.ExpiresIn);
        }
    }
}