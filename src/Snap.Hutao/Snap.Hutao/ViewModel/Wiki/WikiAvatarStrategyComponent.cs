// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Logging;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Notification;
using Windows.System;

namespace Snap.Hutao.ViewModel.Wiki;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class WikiAvatarStrategyComponent
{
    private readonly IAvatarStrategyService avatarStrategyService;
    private readonly CultureOptions cultureOptions;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial WikiAvatarStrategyComponent(IServiceProvider serviceProvider);

    public bool IsBilibiliAvailable { get => cultureOptions.LocaleName is LocaleNames.CHS; }

    [Command("BilibiliStrategyCommand")]
    private static async Task OpenBilibiliStrategyWebsiteAsync(Avatar? avatar)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Open strategy website", "WikiAvatarStrategyComponent.Command", [("target", "bilibili")]));

        if (avatar is null)
        {
            return;
        }

        Uri targetUri = $"https://wiki.biligame.com/ys/{avatar.Name}/攻略".ToUri();
        await Launcher.LaunchUriAsync(targetUri);
    }

    [Command("ChineseStrategyCommand")]
    private async Task OpenChineseStrategyWebsiteAsync(Avatar? avatar)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Open strategy website", "WikiAvatarStrategyComponent.Command", [("target", "miyoushe")]));

        if (avatar is null)
        {
            return;
        }

        AvatarStrategy? strategy = await avatarStrategyService.GetStrategyByAvatarId(avatar.Id).ConfigureAwait(false);

        if (strategy is null)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelWikiAvatarStrategyNotFound));
            return;
        }

        Uri targetUri = strategy.ChineseStrategyUrl;
        if (string.IsNullOrEmpty(targetUri.OriginalString))
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelWikiAvatarStrategyNotFound));
            return;
        }

        await Launcher.LaunchUriAsync(targetUri);
    }

    [Command("OverseaStrategyCommand")]
    private async Task OpenOverseaStrategyWebsiteAsync(Avatar? avatar)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Open strategy website", "WikiAvatarStrategyComponent.Command", [("target", "hoyolab")]));

        if (avatar is null)
        {
            return;
        }

        AvatarStrategy? strategy = await avatarStrategyService.GetStrategyByAvatarId(avatar.Id).ConfigureAwait(false);

        if (strategy is null)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelWikiAvatarStrategyNotFound));
            return;
        }

        Uri targetUri = strategy.OverseaStrategyUrl;
        if (string.IsNullOrEmpty(targetUri.OriginalString))
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelWikiAvatarStrategyNotFound));
            return;
        }

        await Launcher.LaunchUriAsync(targetUri);
    }
}