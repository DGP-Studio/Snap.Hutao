// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.DailyNote.NotifySuppression;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Notification;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.DailyNote;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class DailyNoteNotificationOperation
{
    private const string ToastAttributionUnknown = "Unknown UID";

    private readonly ITaskContext taskContext;
    private readonly DailyNoteOptions options;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial DailyNoteNotificationOperation(IServiceProvider serviceProvider);

    public async ValueTask SendAsync(DailyNoteEntry entry)
    {
        if (entry.DailyNote is null)
        {
            return;
        }

        // This must happen before checking IsAppNotificationEnabled.
        // Always perform check to update dot visibility.
        NotifySuppressionInvoker.Check(entry, out List<DailyNoteNotifyInfo> notifyInfos);

        if (notifyInfos.Count <= 0)
        {
            return;
        }

        if (!HutaoRuntime.IsAppNotificationEnabled)
        {
            return;
        }

        string attribution = entry.UserGameRole?.ToString() ?? ToastAttributionUnknown;

        string reminder = options.IsReminderNotification.Value ? @"scenario=""reminder""" : string.Empty;
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
        AppNotification notification;
        try
        {
            notification = new(rawXml)
            {
                ExpiresOnReboot = true,
            };
        }
        catch (COMException ex)
        {
            ExceptionAttachment.SetAttachment(ex, "raw.xml", rawXml);
            throw;
        }

        if (options.IsSilentWhenPlayingGame.Value && await GameLifeCycle.IsGameRunningAsync(taskContext).ConfigureAwait(false))
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
            ExceptionAttachment.SetAttachment(ex, "RawXml", rawXml);
            messenger.Send(InfoBarMessage.Error(SH.ServiceDailyNoteNotificationSendExceptionTitle, ex));
        }
    }
}