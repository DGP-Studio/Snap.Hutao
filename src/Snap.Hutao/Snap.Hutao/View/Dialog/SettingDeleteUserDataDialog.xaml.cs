// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Windows.Win32.Foundation;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// ɾ����Ϣ�Ի���
/// </summary>
internal sealed partial class SettingDeleteUserDataDialog : ContentDialog
{
    /// <summary>
    /// ����һ���µ�ɾ����Ϣ�Ի���
    /// </summary>
    public SettingDeleteUserDataDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// ��ȡ����İ�ť
    /// </summary>
    /// <returns>����Ľ��</returns>
    public async Task<bool> GetClickButtonResultAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        return result == ContentDialogResult.Primary;
    }
}
