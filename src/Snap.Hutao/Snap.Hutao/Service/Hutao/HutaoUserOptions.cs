// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃用户选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class HutaoUserOptions : ObservableObject, IOptions<HutaoUserOptions>
{
    private string? userName;
    private string? token;

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get => userName; set => SetProperty(ref userName, value); }

    /// <summary>
    /// 访问令牌
    /// </summary>
    public string? Token { get => token; set => SetProperty(ref token, value); }

    /// <inheritdoc/>
    public HutaoUserOptions Value { get => this; }
}