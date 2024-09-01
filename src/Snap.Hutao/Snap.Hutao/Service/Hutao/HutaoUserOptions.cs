// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Service.Hutao;

[Injection(InjectAs.Singleton)]
internal sealed partial class HutaoUserOptions : ObservableObject
{
    private readonly TaskCompletionSource initialization = new();

    private string? userName = SH.ViewServiceHutaoUserLoginOrRegisterHint;
    private bool isLoggedIn;
    private bool isCloudServiceAllowed;
    private bool isLicensedDeveloper;
    private bool isMaintainer;
    private string? gachaLogExpireAt;
    private string? gachaLogExpireAtSlim;
    private string? token;

    public string? UserName { get => userName; set => SetProperty(ref userName, value); }

    public bool IsLoggedIn { get => isLoggedIn; set => SetProperty(ref isLoggedIn, value); }

    public bool IsCloudServiceAllowed { get => isCloudServiceAllowed; set => SetProperty(ref isCloudServiceAllowed, value); }

    public bool IsLicensedDeveloper { get => isLicensedDeveloper; set => SetProperty(ref isLicensedDeveloper, value); }

    public bool IsMaintainer { get => isMaintainer; set => SetProperty(ref isMaintainer, value); }

    public string? GachaLogExpireAt { get => gachaLogExpireAt; set => SetProperty(ref gachaLogExpireAt, value); }

    public string? GachaLogExpireAtSlim { get => gachaLogExpireAtSlim; set => SetProperty(ref gachaLogExpireAtSlim, value); }

    internal string? Token { get => token; set => token = value; }

    internal TaskCompletionSource Initialization { get => initialization; }
}