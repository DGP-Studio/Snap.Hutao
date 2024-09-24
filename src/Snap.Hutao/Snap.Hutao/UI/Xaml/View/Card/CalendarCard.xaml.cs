// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using System.Globalization;

namespace Snap.Hutao.UI.Xaml.View.Card;

internal sealed partial class CalendarCard : Button
{
    public CalendarCard()
    {
        this.InitializeDataContext<ViewModel.Calendar.CalendarViewModel>();
        this.InitializeComponent();
    }
}
