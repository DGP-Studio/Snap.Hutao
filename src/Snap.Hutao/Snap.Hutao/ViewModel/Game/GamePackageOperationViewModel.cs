// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Service.Game.Package;

namespace Snap.Hutao.ViewModel.Game;

internal sealed partial class GamePackageOperationViewModel : ObservableObject
{
    private string title;
    private int progress;
    private bool isIndeterminate;
    private bool isFinished;

    public GamePackageOperationViewModel(string title)
    {
        this.title = title;

        progress = 100 * 45698 / 66842;
    }

    public string Title { get => title; set => SetProperty(ref title, value); }

    public int Progress { get => progress; set => SetProperty(ref progress, value); }

    public bool IsIndeterminate { get => isIndeterminate; set => SetProperty(ref isIndeterminate, value); }

    public bool IsFinished { get => isFinished; set => SetProperty(ref isFinished, value); }

    [Command("CancelCommand")]
    private void Cancel()
    {
        IGamePackageService gamePackageService = Ioc.Default.GetRequiredService<IGamePackageService>();
        gamePackageService.CancelOperationAsync().SafeForget();
        Title = "已取消";
        IsFinished = true;
    }
}
