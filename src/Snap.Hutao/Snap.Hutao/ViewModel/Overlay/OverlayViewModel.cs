// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Game;
using Snap.Hutao.UI.Input.HotKey;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Overlay;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class OverlayViewModel : Abstraction.ViewModel
{
    public partial HotKeyOptions HotKeyOptions { get; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public partial LaunchOptions LaunchOptions { get; }

    public ImmutableArray<OverlayCatalog> Catalogs { get; } =
    [
        new("HotKey", "\uE92E", SH.ViewModelOverlayCatalogHotKeyDisplayName),
        new("Island", "\uEC7A", SH.ViewModelOverlayCatalogIslandSwitchName),
    ];

    public OverlayCatalog? SelectedCatalog { get; set => SetProperty(ref field, value); }

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

    protected override ValueTask<bool> LoadOverrideAsync()
    {
        SelectedCatalog = Catalogs[0];
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