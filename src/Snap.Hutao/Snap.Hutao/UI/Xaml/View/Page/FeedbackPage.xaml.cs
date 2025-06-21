// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.Feedback;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class FeedbackPage : ScopedPage
{
    public FeedbackPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<FeedbackViewModel>();
    }
}