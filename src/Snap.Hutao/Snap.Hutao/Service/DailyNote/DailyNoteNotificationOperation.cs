// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.DailyNote.NotifySuppression;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.DailyNote;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class DailyNoteNotificationOperation
{
    private const string ToastAttributionUnknown = "Unknown";

    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IGameServiceFacade gameService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;
    private readonly DailyNoteOptions options;

    public async ValueTask SendAsync(DailyNoteEntry entry)
    {
        if (!HutaoRuntime.IsAppNotificationEnabled)
        {
            return;
        }

        if (entry.DailyNote is null)
        {
            return;
        }

        NotifySuppressionInvoker.Check(entry, out List<DailyNoteNotifyInfo> notifyInfos);

        if (notifyInfos.Count <= 0)
        {
            return;
        }

        string attribution = entry.UserGameRole is not null
            ? entry.UserGameRole.ToString()
            : await GetUserUidAsync(entry).ConfigureAwait(false);

        string reminder = options.IsReminderNotification ? @"scenario=""reminder""" : string.Empty;
        string content;

        if (notifyInfos.Count > 2)
        {
            string adaptiveSubgroups = string.Join(string.Empty, notifyInfos.Select(info => $"""
                <subgroup>
                    <text hint-align="center">{info.AdaptiveHint}</text>
                    <text hint-style="captionSubtle" hint-align="center">{info.Title}</text>
                </subgroup>
            """));

            content = $"""
                <text>{SH.ServiceDailyNoteNotifierMultiValueReached}</text>
                <group>
                {adaptiveSubgroups}
                </group>
                """;
        }
        else
        {
            content = string.Join(string.Empty, notifyInfos.Select(info => $"""
                <text>{info.Hint}</text>
            """));
        }

        string rawXml = $"""
            <toast {reminder}>
                <header title="{SH.ServiceDailyNoteNotifierTitle}" id="DAILYNOTE" arguments="DAILYNOTE"/>

                <visual>
                    <binding template="ToastGeneric">
                        {content}
                        <text placement="attribution">{attribution}</text>
                    </binding>
                </visual>
                <actions>
                    <action activationType="background" content="{SH.ServiceDailyNoteNotifierActionLaunchGameButton}" arguments="{AppActivation.Action}={AppActivation.LaunchGame};{AppActivation.Uid}={entry.Uid}"/>
                    <action activationType="system" content="{SH.ServiceDailyNoteNotifierActionLaunchGameDismiss}" arguments="dismiss"/>
                </actions>
            </toast>
            """;
        AppNotification notification = new(rawXml);

        if (options.IsSilentWhenPlayingGame && gameService.IsGameRunning())
        {
            notification.SuppressDisplay = true;
        }

        await taskContext.SwitchToMainThreadAsync();
        try
        {
            AppNotificationManager.Default.Show(notification);
        }
        catch (Exception ex)
        {
            ex.AddData("RawXml", rawXml);
            infoBarService.Error(ex, SH.ServiceDailyNoteNotificationSendExceptionTitle);
        }
    }

    private async ValueTask<string> GetUserUidAsync(DailyNoteEntry entry)
    {
        Response<ListWrapper<UserGameRole>> rolesResponse;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            BindingClient bindingClient = scope.ServiceProvider.GetRequiredService<BindingClient>();
            rolesResponse = await bindingClient
                .GetUserGameRolesOverseaAwareAsync(entry.User)
                .ConfigureAwait(false);
        }

        if (rolesResponse.IsOk())
        {
            List<UserGameRole> roles = rolesResponse.Data.List;
            return roles.SingleOrDefault(r => r.GameUid == entry.Uid)?.ToString() ?? ToastAttributionUnknown;
        }

        return SH.ServiceDailyNoteNotifierAttribution;
    }
}