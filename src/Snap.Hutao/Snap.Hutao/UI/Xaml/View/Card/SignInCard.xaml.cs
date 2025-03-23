// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.ViewModel.Sign;

namespace Snap.Hutao.UI.Xaml.View.Card;

internal sealed partial class SignInCard : Button
{
    public SignInCard()
    {
        this.InitializeDataContext<SignInViewModel>();
        InitializeComponent();

        (DataContext as SignInViewModel)?.Initialize(new AwardScrollViewerAccessor(AwardScrollViewer));
    }

    private class AwardScrollViewerAccessor : IAwardScrollViewerAccessor
    {
        public AwardScrollViewerAccessor(ScrollViewer awardScrollViewer)
        {
            AwardScrollViewer = awardScrollViewer;
        }

        public ScrollViewer AwardScrollViewer { get; }
    }
}