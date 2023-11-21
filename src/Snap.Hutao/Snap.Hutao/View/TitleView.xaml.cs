// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Windowing.HotKey;

namespace Snap.Hutao.View;

/// <summary>
/// 标题视图
/// </summary>
[HighQuality]
internal sealed partial class TitleView : UserControl
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
    public string Title
    {
        [SuppressMessage("", "IDE0027")]
        get
        {
#if DEBUG
            return SH.FormatAppDevNameAndVersion(RuntimeOptions.Version);
#else
            return SH.FormatAppNameAndVersion(RuntimeOptions.Version);
#endif
        }
    }

    /// <summary>
    /// 获取可拖动区域
    /// </summary>
    public FrameworkElement DragArea
    {
        get => DragableGrid;
    }

    public RuntimeOptions RuntimeOptions { get; } = Ioc.Default.GetRequiredService<RuntimeOptions>();

    public HotKeyOptions HotKeyOptions { get; } = Ioc.Default.GetRequiredService<HotKeyOptions>();
}
