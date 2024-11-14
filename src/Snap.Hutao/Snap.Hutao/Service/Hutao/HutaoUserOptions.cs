// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Service.Hutao;

[Injection(InjectAs.Singleton)]
internal sealed partial class HutaoUserOptions : ObservableObject
{
    private readonly TaskCompletionSource initialization = new();

    private string? token;

    public string? UserName { get; set => SetProperty(ref field, value); } = SH.ViewServiceHutaoUserLoginOrRegisterHint;

    public bool IsLoggedIn { get; set => SetProperty(ref field, value); }

    public bool IsCloudServiceAllowed { get; set => SetProperty(ref field, value); }

    public bool IsLicensedDeveloper { get; set => SetProperty(ref field, value); }

    public bool IsMaintainer { get; set => SetProperty(ref field, value); }

    public string? GachaLogExpireAt { get; set => SetProperty(ref field, value); }

    public string? GachaLogExpireAtSlim { get; set => SetProperty(ref field, value); }

    internal string? Token { get => token; set => token = value; }

    internal TaskCompletionSource Initialization { get => initialization; }
}