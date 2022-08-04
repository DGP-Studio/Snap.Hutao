// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;

using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 窗体子类管理器
/// </summary>
internal class WindowSubclassManager : IDisposable
{
    private const int SubclassId = 101;

    private const int MinWidth = 848;
    private const int MinHeight = 524;

    private readonly HWND hwnd;

    // We have to explictly hold a reference to the SUBCLASSPROC,
    // otherwise will casuse System.ExecutionEngineException
    private SUBCLASSPROC? subClassProc;

    /// <summary>
    /// 构造一个新的窗体子类管理器
    /// </summary>
    /// <param name="hwnd">窗体句柄</param>
    public WindowSubclassManager(HWND hwnd)
    {
        this.hwnd = hwnd;
    }

    /// <summary>
    /// 尝试设置窗体子类
    /// </summary>
    /// <returns>是否设置成功</returns>
    public bool TrySetWindowSubclass()
    {
        subClassProc = new(OnSubclassProcedure);
        return SetWindowSubclass(hwnd, subClassProc, SubclassId, 0);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        RemoveWindowSubclass(hwnd, subClassProc, SubclassId);
        subClassProc = null;
    }

    private LRESULT OnSubclassProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        switch (uMsg)
        {
            case WM_GETMINMAXINFO:
                {
                    uint dpi = GetDpiForWindow(hwnd);
                    float scalingFactor = dpi / 96f;
                    Win32.Unsafe.SetMinTrackSize(lParam, MinWidth * scalingFactor, MinHeight * scalingFactor);
                    break;
                }
        }

        return DefSubclassProc(hwnd, uMsg, wParam, lParam);
    }
}
