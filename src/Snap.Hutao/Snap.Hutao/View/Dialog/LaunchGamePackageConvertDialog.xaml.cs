// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 启动游戏客户端转换对话框
/// </summary>
public sealed partial class LaunchGamePackageConvertDialog : ContentDialog
{
    private static readonly DependencyProperty DescriptionProperty = Property<LaunchGamePackageConvertDialog>.Depend(nameof(Description), "请稍候");

    /// <summary>
    /// 构造一个新的启动游戏客户端转换对话框
    /// </summary>
    public LaunchGamePackageConvertDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
        DataContext = this;
    }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }
}
