// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 窗体矩形
/// </summary>
internal static class WindowRect
{
    /// <summary>
    /// 获取窗体矩形
    /// </summary>
    /// <returns>矩形</returns>
    public static RECT RetriveWindowRect()
    {
        int left = LocalSetting.GetValueType<int>(SettingKeys.WindowLeft);
        int top = LocalSetting.GetValueType<int>(SettingKeys.WindowTop);
        int right = LocalSetting.GetValueType<int>(SettingKeys.WindowRight);
        int bottom = LocalSetting.GetValueType<int>(SettingKeys.WindowBottom);

        return new(left, top, right, bottom);
    }

    /// <summary>
    /// 保存窗体矩形
    /// </summary>
    /// <param name="handle">窗体句柄</param>
    public static void SaveWindowRect(HWND handle)
    {
        WINDOWPLACEMENT windowPlacement = WINDOWPLACEMENT.Default;

        GetWindowPlacement(handle, ref windowPlacement);

        LocalSetting.Set(SettingKeys.WindowLeft, windowPlacement.rcNormalPosition.left);
        LocalSetting.Set(SettingKeys.WindowTop, windowPlacement.rcNormalPosition.top);
        LocalSetting.Set(SettingKeys.WindowRight, windowPlacement.rcNormalPosition.right);
        LocalSetting.Set(SettingKeys.WindowBottom, windowPlacement.rcNormalPosition.bottom);
    }
}