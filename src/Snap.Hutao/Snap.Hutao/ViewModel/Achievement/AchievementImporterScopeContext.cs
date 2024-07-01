// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.ViewModel.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementImporterScopeContext
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IClipboardProvider clipboardProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public IFileSystemPickerInteraction FileSystemPickerInteraction { get => fileSystemPickerInteraction; }

    public JsonSerializerOptions JsonSerializerOptions { get => jsonSerializerOptions; }

    public IContentDialogFactory ContentDialogFactory { get => contentDialogFactory; }

    public IClipboardProvider ClipboardProvider { get => clipboardProvider; }

    public IInfoBarService InfoBarService { get => infoBarService; }

    public ITaskContext TaskContext { get => taskContext; }
}