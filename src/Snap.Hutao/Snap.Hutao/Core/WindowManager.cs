// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Control.HostBackdrop;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using WinRT.Interop;

namespace Snap.Hutao.Core;

/// <summary>
/// 窗口状态管理器
/// 主要包含了各类 P/Inoke 代码
/// </summary>
internal class WindowManager
{
    private const int MinWidth = 800;
    private const int MinHeight = 600;

    private readonly HWND handle;
    private readonly Window window;
    private readonly UIElement titleBar;
    private readonly ILogger<WindowManager> logger;

    // We have to explictly hold a reference to the SUBCLASSPROC
    // otherwise will casuse System.ExecutionEngineException
    private SUBCLASSPROC? subClassProc;

    /// <summary>
    /// 构造一个新的窗口状态管理器
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="titleBar">充当标题栏的元素</param>
    public WindowManager(Window window, UIElement titleBar)
    {
        this.window = window;
        this.titleBar = titleBar;
        logger = Ioc.Default.GetRequiredService<ILogger<WindowManager>>();
        handle = (HWND)WindowNative.GetWindowHandle(window);
        InitializeWindow();
    }

    private static RECT RetriveWindowRect()
    {
        int left = LocalSetting.GetValueType<int>(SettingKeys.WindowLeft);
        int top = LocalSetting.GetValueType<int>(SettingKeys.WindowTop);
        int right = LocalSetting.GetValueType<int>(SettingKeys.WindowRight);
        int bottom = LocalSetting.GetValueType<int>(SettingKeys.WindowBottom);

        return new() { left = left, top = top, right = right, bottom = bottom };
    }

    private static void SaveWindowRect(HWND handle)
    {
        WINDOWPLACEMENT windowPlacement = new()
        {
            length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
        };

        PInvoke.GetWindowPlacement(handle, ref windowPlacement);

        LocalSetting.Set(SettingKeys.WindowLeft, windowPlacement.rcNormalPosition.left);
        LocalSetting.Set(SettingKeys.WindowTop, windowPlacement.rcNormalPosition.top);
        LocalSetting.Set(SettingKeys.WindowRight, windowPlacement.rcNormalPosition.right);
        LocalSetting.Set(SettingKeys.WindowBottom, windowPlacement.rcNormalPosition.bottom);
    }

    private void InitializeWindow()
    {
        window.ExtendsContentIntoTitleBar = true;
        window.SetTitleBar(titleBar);
        window.Closed += OnWindowClosed;

        PInvoke.SetWindowText(handle, "胡桃");
        RECT rect = RetriveWindowRect();
        if ((rect.right - rect.left) * (rect.bottom - rect.top) > 0)
        {
            WINDOWPLACEMENT windowPlacement = new()
            {
                length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
                showCmd = SHOW_WINDOW_CMD.SW_SHOWNORMAL,
                ptMaxPosition = new() { x = -1, y = -1 },
                rcNormalPosition = rect,
            };

            PInvoke.SetWindowPlacement(handle, in windowPlacement);
        }

        bool micaApplied = new SystemBackdrop(window).TrySetBackdrop();
        logger.LogInformation(EventIds.BackdropState, "Apply {name} : {result}", nameof(SystemBackdrop), micaApplied ? "succeed" : "failed");

        subClassProc = new(OnWindowProcedure);
        _ = PInvoke.SetWindowSubclass(handle, subClassProc, 101, 0);
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        PInvoke.RemoveWindowSubclass(handle, subClassProc, 101);
        subClassProc = null;
        SaveWindowRect(handle);
    }

    private LRESULT OnWindowProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        switch (uMsg)
        {
            case PInvoke.WM_GETMINMAXINFO:
                {
                    uint dpi = PInvoke.GetDpiForWindow(handle);
                    float scalingFactor = dpi / 96f;

                    MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                    minMaxInfo.ptMinTrackSize.x = (int)Math.Max(MinWidth * scalingFactor, minMaxInfo.ptMinTrackSize.x);
                    minMaxInfo.ptMinTrackSize.y = (int)Math.Max(MinHeight * scalingFactor, minMaxInfo.ptMinTrackSize.y);
                    Marshal.StructureToPtr(minMaxInfo, lParam, true);
                    break;
                }
        }

        return PInvoke.DefSubclassProc(hwnd, uMsg, wParam, lParam);
    }
}