// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Service.Yae;

namespace Snap.Hutao.ViewModel.Achievement;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class AchievementImporterScopeContext
{
    [GeneratedConstructor]
    public partial AchievementImporterScopeContext(IServiceProvider serviceProvider);

    public partial IFileSystemPickerInteraction FileSystemPickerInteraction { get; }

    public partial JsonSerializerOptions JsonSerializerOptions { get; }

    public partial IContentDialogFactory ContentDialogFactory { get; }

    public partial IClipboardProvider ClipboardProvider { get; }

    public partial IServiceProvider ServiceProvider { get; }

    public partial IYaeService YaeService { get; }

    public partial IMessenger Messenger { get; }
}