// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Snap.Hutao.ViewModel.Setting;

internal sealed class SettingHomeCardObservableCollection : ObservableCollection<SettingHomeCardViewModel>
{
    public SettingHomeCardObservableCollection(List<SettingHomeCardViewModel> items)
        : base(items)
    {
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        foreach ((int index, SettingHomeCardViewModel item) in Items.Index())
        {
            item.Order = index;
        }
    }
}