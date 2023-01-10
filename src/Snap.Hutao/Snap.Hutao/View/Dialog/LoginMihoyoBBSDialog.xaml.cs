// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Web.Hoyolab.Passport;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 登录到米游社对话框
/// </summary>
public sealed partial class LoginMihoyoBBSDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的登录到米游社对话框
    /// </summary>
    /// <param name="window">窗体</param>
    public LoginMihoyoBBSDialog()
    {
        InitializeComponent();
        XamlRoot = Ioc.Default.GetRequiredService<MainWindow>().Content.XamlRoot;
    }

    /// <summary>
    /// 异步获取用户输入的账号密码
    /// </summary>
    /// <returns>账号密码</returns>
    public async Task<ValueResult<bool, Dictionary<string, string>>> GetInputAccountPasswordAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        bool result = await ShowAsync() == ContentDialogResult.Primary;

        Dictionary<string, string> data = new()
        {
            { "account", RSAEncryptedString.Encrypt(AccountTextBox.Text) },
            { "password", RSAEncryptedString.Encrypt(PasswordTextBox.Password) },
        };

        return new(result, data);
    }
}
