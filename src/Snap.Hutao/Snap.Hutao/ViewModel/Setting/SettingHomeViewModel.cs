// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingHomeViewModel : Abstraction.ViewModel
{
    private readonly HomeCardOptions homeCardOptions = new();
    private readonly AppOptions appOptions;

    public HomeCardOptions HomeCardOptions { get => homeCardOptions; }

    public AppOptions AppOptions { get => appOptions; }

    public NameValue<Region>? SelectedRegion
    {
        get => field ??= AppOptions.GetCurrentRegionForSelectionOrDefault();
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.Region = value.Value;
            }
        }
    }
}