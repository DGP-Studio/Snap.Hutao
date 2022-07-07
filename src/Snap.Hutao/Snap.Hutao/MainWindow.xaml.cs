// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Control.HostBackdrop;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Win32;
using System.Drawing;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace Snap.Hutao;

/// <summary>
/// 主窗体
/// </summary>
[Injection(InjectAs.Singleton)]
public sealed partial class MainWindow : Window
{
    private readonly AppDbContext appDbContext;
    private readonly ILogger<MainWindow> logger;
    private readonly IntPtr handle;

    /// <summary>
    /// 构造一个新的主窗体
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="logger">日志器</param>
    public MainWindow(AppDbContext appDbContext, ILogger<MainWindow> logger)
    {
        this.appDbContext = appDbContext;
        this.logger = logger;

        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(TitleBarView.DragableArea);

        handle = WindowNative.GetWindowHandle(this);
        InitializeWindow();
    }

    private static RECT RetriveWindowRect()
    {
        int left = LocalSetting.GetValueType<int>(SettingKeys.WindowLeft);
        int top = LocalSetting.GetValueType<int>(SettingKeys.WindowTop);
        int right = LocalSetting.GetValueType<int>(SettingKeys.WindowRight);
        int bottom = LocalSetting.GetValueType<int>(SettingKeys.WindowBottom);

        return new RECT(left, top, right, bottom);
    }

    private void InitializeWindow()
    {
        RECT rect = RetriveWindowRect();
        if (rect.Size.IsEmpty)
        {
            return;
        }

        WINDOWPLACEMENT windowPlacement = new()
        {
            Length = Marshal.SizeOf<WINDOWPLACEMENT>(),
            MaxPosition = new Point(-1, -1),
            NormalPosition = rect,
            ShowCmd = ShowWindowCommand.Normal,
        };

        User32.SetWindowPlacement(handle, ref windowPlacement);
        User32.SetWindowText(handle, "胡桃");

        bool micaApplied = new SystemBackdrop(this).TrySetBackdrop();
        logger.LogInformation("{name} 设置{result}", nameof(SystemBackdrop), micaApplied ? "成功" : "失败");
    }

    private void SaveWindowRect()
    {
        WINDOWPLACEMENT windowPlacement = WINDOWPLACEMENT.Default;
        User32.GetWindowPlacement(handle, ref windowPlacement);

        LocalSetting.SetValueType(SettingKeys.WindowLeft, windowPlacement.NormalPosition.Left);
        LocalSetting.SetValueType(SettingKeys.WindowTop, windowPlacement.NormalPosition.Top);
        LocalSetting.SetValueType(SettingKeys.WindowRight, windowPlacement.NormalPosition.Right);
        LocalSetting.SetValueType(SettingKeys.WindowBottom, windowPlacement.NormalPosition.Bottom);
    }

    private void MainWindowClosed(object sender, WindowEventArgs args)
    {
        SaveWindowRect();

        // save datebase
        int changes = appDbContext.SaveChanges();
        Verify.Operation(changes == 0, "存在可避免的未经处理的数据库更改");
    }
}