// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Snap.Hutao.Model.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 实时便笺通知设置对话框
/// </summary>
public sealed partial class DailyNoteNotificationDialog : ContentDialog
{
    /// <summary>
    /// 构造一个新的实时便笺通知设置对话框
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="entry">实时便笺</param>
    public DailyNoteNotificationDialog(Window window, DailyNoteEntry entry)
    {
        InitializeComponent();
        XamlRoot = window.Content.XamlRoot;
        DataContext = entry;
    }
}
