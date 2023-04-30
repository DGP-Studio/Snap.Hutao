// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Service.Game.Package;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 启动游戏客户端转换对话框
/// </summary>
[HighQuality]
internal sealed partial class LaunchGamePackageConvertDialog : ContentDialog
{
    private static readonly DependencyProperty StateProperty = Property<LaunchGamePackageConvertDialog>.Depend<PackageReplaceStatus>(nameof(State));

    /// <summary>
    /// 构造一个新的启动游戏客户端转换对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public LaunchGamePackageConvertDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        XamlRoot = serviceProvider.GetRequiredService<MainWindow>().Content.XamlRoot;

        DataContext = this;
    }

    /// <summary>
    /// 描述
    /// </summary>
    public PackageReplaceStatus State
    {
        get => (PackageReplaceStatus)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }
}
