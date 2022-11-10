// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 角色信息查询UID对话框
/// </summary>
public sealed partial class AvatarInfoQueryDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的角色信息查询UID对话框
    /// </summary>
    /// <param name="window">窗口</param>
    public AvatarInfoQueryDialog(Window window)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
    }

    /// <summary>
    /// 获取玩家UID
    /// </summary>
    /// <returns>玩家UID</returns>
    public async Task<ValueResult<bool, PlayerUid>> GetPlayerUidAsync()
    {
        ContentDialogResult result = await ShowAsync();

        return new(result == ContentDialogResult.Primary, result == ContentDialogResult.Primary ? new(InputText.Text) : default);
    }

    private void InputTextChanged(object sender, TextChangedEventArgs e)
    {
        bool inputValid = string.IsNullOrEmpty(InputText.Text) && InputText.Text.Length == 9;

        (PrimaryButtonText, IsPrimaryButtonEnabled) = inputValid switch
        {
            true => ("请输入正确的UID", false),
            false => ("确认", true),
        };
    }
}
