// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Web.Hutao;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Hutao;

internal static class HutaoUserOptionsExtension
{
    public static string? GetActualUserName(this HutaoUserOptions options)
    {
        return options.IsLoggedIn ? options.UserName : null;
    }

    public static async ValueTask<string?> GetTokenAsync(this HutaoUserOptions options)
    {
        await options.Initialization.Task.ConfigureAwait(false);
        return options.Token;
    }

    public static async ValueTask<bool> PostLoginSucceedAsync(this HutaoUserOptions options, HutaoPassportClient passportClient, ITaskContext taskContext, string username, string password, string? token)
    {
        LocalSetting.Set(SettingKeys.PassportUserName, username);
        LocalSetting.Set(SettingKeys.PassportPassword, password);

        await taskContext.SwitchToMainThreadAsync();
        options.UserName = username;
        options.Token = token;
        options.IsLoggedIn = true;
        options.Initialization.TrySetResult();

        await taskContext.SwitchToBackgroundAsync();
        Web.Response.Response<UserInfo> userInfoResponse = await passportClient.GetUserInfoAsync().ConfigureAwait(false);
        if (userInfoResponse.IsOk())
        {
            await taskContext.SwitchToMainThreadAsync();
            UpdateUserInfo(options, userInfoResponse.Data);
            return true;
        }

        return false;

        static void UpdateUserInfo(HutaoUserOptions options, UserInfo userInfo)
        {
            options.IsLicensedDeveloper = userInfo.IsLicensedDeveloper;
            options.IsMaintainer = userInfo.IsMaintainer;

            options.IsCloudServiceAllowed = options.IsLicensedDeveloper || userInfo.GachaLogExpireAt > DateTimeOffset.UtcNow;
            string unescaped = Regex.Unescape(SH.ServiceHutaoUserGachaLogExpiredAt);
            options.GachaLogExpireAt = string.Format(CultureInfo.CurrentCulture, unescaped, userInfo.GachaLogExpireAt);
            options.GachaLogExpireAtSlim = $"{userInfo.GachaLogExpireAt:yyyy.MM.dd HH:mm:ss}";
        }
    }

    public static void PostLogoutOrUnregister(this HutaoUserOptions options)
    {
        LocalSetting.Set(SettingKeys.PassportUserName, string.Empty);
        LocalSetting.Set(SettingKeys.PassportPassword, string.Empty);

        options.UserName = null;
        options.Token = null;
        options.IsLoggedIn = false;
        ClearUserInfo(options);

        static void ClearUserInfo(HutaoUserOptions options)
        {
            options.IsLicensedDeveloper = false;
            options.IsMaintainer = false;
            options.GachaLogExpireAt = null;
            options.GachaLogExpireAtSlim = null;
            options.IsCloudServiceAllowed = false;
        }
    }

    public static void PostLoginFailed(this HutaoUserOptions options)
    {
        options.UserName = SH.ViewServiceHutaoUserLoginFailHint;
        options.Initialization.TrySetResult();
    }

    public static void PostLoginSkipped(this HutaoUserOptions options)
    {
        options.Initialization.TrySetResult();
    }
}