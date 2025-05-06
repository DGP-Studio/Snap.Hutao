// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Behavior;

internal sealed class UninitializeDataContextOnUnloadedBehavior : BehaviorBase<FrameworkElement>
{
    protected override bool Uninitialize()
    {
        if (AssociatedObject.DataContext<ViewModel.Abstraction.IViewModel>() is { } viewModel)
        {
            // Wait to ensure viewmodel operation is completed
            using (viewModel.DisposeLock.Enter())
            {
                viewModel.Uninitialize();
            }
        }

        return true;
    }
}