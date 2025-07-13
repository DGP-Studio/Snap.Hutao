// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IHomaPassportEndpoints : IHomaRootAccess
{
    public string PassportVerify()
    {
        return $"{Root}/Passport/v2/Verify";
    }

    public string PassportRegister()
    {
        return $"{Root}/Passport/v2/Register";
    }

    public string PassportCancel()
    {
        return $"{Root}/Passport/v2/Cancel";
    }

    public string PassportResetUserName()
    {
        return $"{Root}/Passport/v2/ResetUsername";
    }

    public string PassportResetPassword()
    {
        return $"{Root}/Passport/v2/ResetPassword";
    }

    public string PassportLogin()
    {
        return $"{Root}/Passport/v2/Login";
    }

    public string PassportUserInfo()
    {
        return $"{Root}/Passport/v2/UserInfo";
    }

    public string PassportRefreshToken()
    {
        return $"{Root}/Passport/v2/RefreshToken";
    }

    public string PassportRevokeToken()
    {
        return $"{Root}/Passport/v2/RevokeToken";
    }

    public string PassportRevokeAllTokens()
    {
        return $"{Root}/Passport/v2/RevokeAllTokens";
    }
}