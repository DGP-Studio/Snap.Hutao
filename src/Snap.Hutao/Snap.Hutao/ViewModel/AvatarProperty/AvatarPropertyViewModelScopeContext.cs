// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DataTransfer;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;

namespace Snap.Hutao.ViewModel.AvatarProperty;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AvatarPropertyViewModelScopeContext
{
    public partial IContentDialogFactory ContentDialogFactory { get; }

    public partial IServiceScopeFactory ServiceScopeFactory { get; }

    public partial ICultivationService CultivationService { get; }

    public partial IAvatarInfoService AvatarInfoService { get; }

    public partial IClipboardProvider ClipboardProvider { get; }

    public partial IInfoBarService InfoBarService { get; }

    public partial ITaskContext TaskContext { get; }

    public partial IUserService UserService { get; }
}