// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Service.Game;
using Snap.Hutao.UI.Input.HotKey;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Overlay;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class OverlayViewModel : Abstraction.ViewModel
{
    [GeneratedConstructor]
    public partial OverlayViewModel(IServiceProvider serviceProvider);

    public partial HotKeyOptions HotKeyOptions { get; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public partial LaunchOptions LaunchOptions { get; }

    public ImmutableArray<OverlayCatalog> Catalogs { get; } =
    [
        new("HotKey", "\uE92E", SH.ViewModelOverlayCatalogHotKeyDisplayName),
        new("Island", "\uEC7A", SH.ViewModelOverlayCatalogIslandSwitchName),
    ];

    public OverlayCatalog? SelectedCatalog
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                LocalSetting.Set(SettingKeys.OverlaySelectedCatalogId, field?.Id);
            }
        }
    }

    internal void HandleMouseWheel(int lines)
    {
        if (lines is 0)
        {
            return;
        }

        if (SelectedCatalog is null)
        {
            return;
        }

        int index = Catalogs.IndexOf(SelectedCatalog);
        if (lines < 0)
        {
            index++;
        }
        else
        {
            index--;
        }

        index = (index + Catalogs.Length) % Catalogs.Length;
        SelectedCatalog = Catalogs[index];
    }

    protected override ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        string selectedId = LocalSetting.Get(SettingKeys.OverlaySelectedCatalogId, "HotKey");
        SelectedCatalog = Catalogs.SingleOrDefault(c => c.Id == selectedId);
        return ValueTask.FromResult(true);
    }

    [Command("SwitchToNextCatalogCommand")]
    private void SwitchToNextCatalog()
    {
        if (SelectedCatalog is null)
        {
            return;
        }

        int index = Catalogs.IndexOf(SelectedCatalog);
        index++;
        index = (index + Catalogs.Length) % Catalogs.Length;
        SelectedCatalog = Catalogs[index];
    }
}