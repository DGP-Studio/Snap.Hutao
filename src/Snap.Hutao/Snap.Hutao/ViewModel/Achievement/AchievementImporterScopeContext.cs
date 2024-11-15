// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.ViewModel.Achievement;

[ConstructorGenerated(InjectToPropertiesInstead = true)]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementImporterScopeContext
{
    public partial IFileSystemPickerInteraction FileSystemPickerInteraction { get; }

    public partial JsonSerializerOptions JsonSerializerOptions { get; }

    public partial IContentDialogFactory ContentDialogFactory { get; }

    public partial IClipboardProvider ClipboardProvider { get; }

    public partial IInfoBarService InfoBarService { get; }

    public partial ITaskContext TaskContext { get; }
}