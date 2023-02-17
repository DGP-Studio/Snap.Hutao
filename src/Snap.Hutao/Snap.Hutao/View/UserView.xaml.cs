// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.View;

/// <summary>
/// 用户视图
/// </summary>
[HighQuality]
internal sealed partial class UserView : UserControl
{
    /// <summary>
    /// 构造一个新的用户视图
    /// </summary>
    public UserView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<UserViewModel>();
    }
}
