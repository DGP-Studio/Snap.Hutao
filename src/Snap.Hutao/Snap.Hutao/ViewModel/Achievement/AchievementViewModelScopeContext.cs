// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.ViewModel.Game;

namespace Snap.Hutao.ViewModel.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementViewModelScopeContext
{
    public partial IFileSystemPickerInteraction FileSystemPickerInteraction { get; }

    public partial ILogger<AchievementViewModelScopeContext> Logger { get; }

    public partial JsonSerializerOptions JsonSerializerOptions { get; }

    public partial IContentDialogFactory ContentDialogFactory { get; }

    public partial AchievementImporter AchievementImporter { get; }

    public partial LaunchGameViewModel LaunchGameViewModel { get; }

    public partial IAchievementService AchievementService { get; }

    public partial IClipboardProvider ClipboardProvider { get; }

    public partial IServiceProvider ServiceProvider { get; }

    public partial IMetadataService MetadataService { get; }

    public partial IInfoBarService InfoBarService { get; }

    public partial ITaskContext TaskContext { get; }
}