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
    [SuppressMessage("", "CA1822")]
    public string Title
    {
        get
        {
            Core.RuntimeOptions hutaoOptions = Ioc.Default.GetRequiredService<Core.RuntimeOptions>();

            string format =
#if DEBUG
                SH.AppDevNameAndVersion;
#else
                SH.AppNameAndVersion;
#endif
            return format.Format(hutaoOptions.Version);
        }
    }

    /// <summary>
    /// 获取可拖动区域
    /// </summary>
    public FrameworkElement DragArea
    {
        get => DragableGrid;
    }

    [SuppressMessage("", "CA1822")]
    public RuntimeOptions RuntimeOptions
    {
        get => Ioc.Default.GetRequiredService<RuntimeOptions>();
    }

    [SuppressMessage("", "CA1822")]
    public HotKeyOptions HotKeyOptions
    {
        get => Ioc.Default.GetRequiredService<HotKeyOptions>();
    }
}
