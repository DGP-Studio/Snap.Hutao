// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View;

/// <summary>
/// 标题视图
/// </summary>
public sealed partial class TitleView : UserControl
{
    /// <summary>
    /// 构造一个新的标题视图
    /// </summary>
    public TitleView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 标题
    /// </summary>
    [SuppressMessage("", "CA1822")]
    public string Title
    {
#if DEBUG
        get => $"胡桃 Dev Build";
#else
        get => $"胡桃 {Core.CoreEnvironment.Version}";
#endif
    }

    /// <summary>
    /// 获取可拖动区域
    /// </summary>
    public FrameworkElement DragArea
    {
        get => DragableGrid;
    }
}
