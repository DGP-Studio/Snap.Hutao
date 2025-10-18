// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel.Abstraction;
using Snap.Hutao.ViewModel.Sign;

namespace Snap.Hutao.UI.Xaml.View.Card;

internal sealed partial class SignInCard : Button
{
    public SignInCard(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        this.InitializeViewModelSlim<SignInViewModel>(serviceProvider);
        this.DataContext<SignInViewModel>()?.AttachXamlElement(AwardScrollViewer);
    }
}