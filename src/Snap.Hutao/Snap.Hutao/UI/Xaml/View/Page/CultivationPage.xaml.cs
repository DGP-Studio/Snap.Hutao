// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.ViewModel.Cultivation;

namespace Snap.Hutao.UI.Xaml.View.Page;

internal sealed partial class CultivationPage : ScopedPage
{
    public CultivationPage()
    {
        InitializeWith<CultivationViewModel>();
        InitializeComponent();

        (DataContext as CultivationViewModel)?.Initialize(new CultivateEntryItemsRepeaterAccessor(CultivateEntryItemsRepeater));
    }

    private class CultivateEntryItemsRepeaterAccessor : ICultivateEntryItemsRepeaterAccessor
    {
        public CultivateEntryItemsRepeaterAccessor(ItemsRepeater cultivateEntryItemsRepeater)
        {
            CultivateEntryItemsRepeater = cultivateEntryItemsRepeater;
        }

        public ItemsRepeater CultivateEntryItemsRepeater { get; }
    }
}