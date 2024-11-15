﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class TestPage : ScopedPage
{
    public TestPage()
    {
        InitializeWith<TestViewModel>();
        InitializeComponent();
    }
}