// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint;

internal interface IHomaPassportEndpoints : IHomaRootAccess
{
    public string PassportVerify()
    {
        return $"{Root}/Passport/Verify";
    }

    public string PassportRegister()
    {
        return $"{Root}/Passport/Register";
    }

    public string PassportCancel()
    {
        return $"{Root}/Passport/Cancel";
    }

    public string PassportResetPassword()
    {
        return $"{Root}/Passport/ResetPassword";
    }

    public string PassportLogin()
    {
        return $"{Root}/Passport/Login";
    }

    public string PassportUserInfo()
    {
        return $"{Root}/Passport/UserInfo";
    }
}