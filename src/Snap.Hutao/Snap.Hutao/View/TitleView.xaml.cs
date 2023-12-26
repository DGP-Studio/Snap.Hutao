// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Windowing.HotKey;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Update;

namespace Snap.Hutao.View;

/// <summary>
/// 标题视图
/// </summary>
[HighQuality]
[INotifyPropertyChanged]
internal sealed partial class TitleView : UserControl
{
    private readonly CancellationTokenSource checkUpdateTaskCancellationTokenSource = new();
    private UpdateStatus? updateStatus;

    public TitleView()
    {
        Loaded += OnTitleViewLoaded;
        Unloaded += OnTitleViewUnloaded;
        InitializeComponent();
    }

    public string Title
    {
        [SuppressMessage("", "IDE0027")]
        get
        {
#if DEBUG
            return SH.FormatAppDevNameAndVersion(RuntimeOptions.Version);
#else
            return SH.FormatAppNameAndVersion(RuntimeOptions.Version);
#endif
        }
    }

    public FrameworkElement DragArea
    {
        get => DragableGrid;
    }

    public RuntimeOptions RuntimeOptions { get; } = Ioc.Default.GetRequiredService<RuntimeOptions>();

    public HotKeyOptions HotKeyOptions { get; } = Ioc.Default.GetRequiredService<HotKeyOptions>();

    public UpdateStatus? UpdateStatus { get => updateStatus; set => SetProperty(ref updateStatus, value); }

    private void OnTitleViewLoaded(object sender, RoutedEventArgs e)
    {
        DoCheckUpdateAsync(checkUpdateTaskCancellationTokenSource.Token).SafeForget();
        Loaded -= OnTitleViewLoaded;
    }

    private void OnTitleViewUnloaded(object sender, RoutedEventArgs e)
    {
        checkUpdateTaskCancellationTokenSource.Cancel();
        Unloaded -= OnTitleViewUnloaded;
    }

    private async ValueTask DoCheckUpdateAsync(CancellationToken token)
    {
        IServiceProvider serviceProvider = Ioc.Default;
        IUpdateService updateService = serviceProvider.GetRequiredService<IUpdateService>();

        IProgressFactory progressFactory = serviceProvider.GetRequiredService<IProgressFactory>();
        IProgress<UpdateStatus> progress = progressFactory.CreateForMainThread<UpdateStatus>(status => UpdateStatus = status);
        if (await updateService.CheckForUpdateAndDownloadAsync(progress, token).ConfigureAwait(false))
        {
            ContentDialogResult result = await serviceProvider
                .GetRequiredService<IContentDialogFactory>()
                .CreateForConfirmCancelAsync(
                    SH.FormatViewTitileUpdatePackageReadyTitle(UpdateStatus?.Version),
                    SH.ViewTitileUpdatePackageReadyContent,
                    ContentDialogButton.Primary)
                .ConfigureAwait(false);
            if (result == ContentDialogResult.Primary)
            {
                updateService.LaunchInstaller();
            }
        }

        await serviceProvider.GetRequiredService<ITaskContext>().SwitchToMainThreadAsync();
        UpdateStatus = null;
    }
}