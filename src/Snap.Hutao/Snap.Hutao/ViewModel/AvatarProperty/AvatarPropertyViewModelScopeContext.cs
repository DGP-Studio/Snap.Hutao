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
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ICultivationService cultivationService;
    private readonly IAvatarInfoService avatarInfoService;
    private readonly IClipboardProvider clipboardProvider;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    public IContentDialogFactory ContentDialogFactory { get => contentDialogFactory; }

    public IServiceScopeFactory ServiceScopeFactory { get => serviceScopeFactory; }

    public ICultivationService CultivationService { get => cultivationService; }

    public IAvatarInfoService AvatarInfoService { get => avatarInfoService; }

    public IClipboardProvider ClipboardProvider { get => clipboardProvider; }

    public IInfoBarService InfoBarService { get => infoBarService; }

    public ITaskContext TaskContext { get => taskContext; }

    public IUserService UserService { get => userService; }
}