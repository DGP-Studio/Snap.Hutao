// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Game.Package;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 启动游戏客户端转换对话框
/// </summary>
[HighQuality]
[DependencyProperty("State", typeof(PackageReplaceStatus))]
internal sealed partial class LaunchGamePackageConvertDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的启动游戏客户端转换对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public LaunchGamePackageConvertDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        DataContext = this;
    }
}
