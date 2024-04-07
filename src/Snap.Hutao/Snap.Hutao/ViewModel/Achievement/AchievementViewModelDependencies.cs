// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.ViewModel.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementViewModelDependencies
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly AchievementImporter achievementImporter;
    private readonly IAchievementService achievementService;
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly ITaskContext taskContext;

    public IFileSystemPickerInteraction FileSystemPickerInteraction { get => fileSystemPickerInteraction; }

    public JsonSerializerOptions JsonSerializerOptions { get => jsonSerializerOptions; }

    public IContentDialogFactory ContentDialogFactory { get => contentDialogFactory; }

    public AchievementImporter AchievementImporter { get => achievementImporter; }

    public IAchievementService AchievementService { get => achievementService; }

    public IMetadataService MetadataService { get => metadataService; }

    public IInfoBarService InfoBarService { get => infoBarService; }

    public ITaskContext TaskContext { get => taskContext; }
}