// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Snap.Hutao.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Snap.Hutao.View.Page;

/// <summary>
/// 角色资料页
/// </summary>
public sealed partial class WikiAvatarPage : Microsoft.UI.Xaml.Controls.Page
{
    /// <summary>
    /// 构造一个新的角色资料页
    /// </summary>
    public WikiAvatarPage()
    {
        DataContext = Ioc.Default.GetRequiredService<WikiAvatarViewModel>();
        InitializeComponent();
    }
}
