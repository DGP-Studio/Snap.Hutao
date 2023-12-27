// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Windowing.HotKey;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Update;
using System.Globalization;
using System.Text;

namespace Snap.Hutao.ViewModel;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class TitleViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IProgressFactory progressFactory;
    private readonly RuntimeOptions runtimeOptions;
    private readonly HotKeyOptions hotKeyOptions;
    private readonly IUpdateService updateService;
    private readonly ITaskContext taskContext;

    private UpdateStatus? updateStatus;

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public HotKeyOptions HotKeyOptions { get => hotKeyOptions; }

    public string Title
    {
        [SuppressMessage("", "IDE0027")]
        get
        {
            string name = new StringBuilder()
                .Append("App")
                .AppendIf(runtimeOptions.IsElevated, "Elevated")
#if DEBUG
                .Append("Dev")
#endif
                .Append("NameAndVersion")
                .ToString();

            string? format = SH.GetString(CultureInfo.CurrentCulture, name);
            ArgumentException.ThrowIfNullOrEmpty(format);
            return string.Format(CultureInfo.CurrentCulture, format, runtimeOptions.Version);
        }
    }

    public UpdateStatus? UpdateStatus { get => updateStatus; set => SetProperty(ref updateStatus, value); }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        await DoCheckUpdateAsync().ConfigureAwait(false);
        return true;
    }

    private async ValueTask DoCheckUpdateAsync()
    {
        IProgress<UpdateStatus> progress = progressFactory.CreateForMainThread<UpdateStatus>(status => UpdateStatus = status);
        if (await updateService.CheckForUpdateAndDownloadAsync(progress).ConfigureAwait(false))
        {
            ContentDialogResult result = await contentDialogFactory
                .CreateForConfirmCancelAsync(
                    SH.FormatViewTitileUpdatePackageReadyTitle(UpdateStatus?.Version),
                    SH.ViewTitileUpdatePackageReadyContent,
                    ContentDialogButton.Primary)
                .ConfigureAwait(false);
            if (result == ContentDialogResult.Primary)
            {
                await updateService.LaunchUpdaterAsync().ConfigureAwait(false);
            }
        }

        await taskContext.SwitchToMainThreadAsync();
        UpdateStatus = null;
    }
}