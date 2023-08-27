// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺通知器
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class DailyNoteNotificationOperation
{
    private const string ToastHeaderIdArgument = "DAILYNOTE";
    private const string ToastAttributionUnknown = "Unknown";

    private readonly ITaskContext taskContext;
    private readonly IGameService gameService;
    private readonly BindingClient bindingClient;
    private readonly DailyNoteOptions options;

    public async ValueTask SendAsync(DailyNoteEntry entry)
    {
        if (entry.DailyNote is null)
        {
            return;
        }

        List<DailyNoteNotifyInfo> notifyInfos = new();

        CheckNotifySuppressed(entry, notifyInfos);

        if (notifyInfos.Count <= 0)
        {
            return;
        }

        string? attribution = SH.ServiceDailyNoteNotifierAttribution;

        if (entry.UserGameRole is not null)
        {
            attribution = entry.UserGameRole.ToString();
        }
        else
        {
            Response<ListWrapper<UserGameRole>> rolesResponse = await bindingClient
                .GetUserGameRolesOverseaAwareAsync(entry.User)
                .ConfigureAwait(false);

            if (rolesResponse.IsOk())
            {
                List<UserGameRole> roles = rolesResponse.Data.List;
                attribution = roles.SingleOrDefault(r => r.GameUid == entry.Uid)?.ToString() ?? ToastAttributionUnknown;
            }
        }

        ToastContentBuilder builder = new ToastContentBuilder()
            .AddHeader(ToastHeaderIdArgument, SH.ServiceDailyNoteNotifierTitle, ToastHeaderIdArgument)
            .AddAttributionText(attribution)
            .AddButton(new ToastButton()
                .SetContent(SH.ServiceDailyNoteNotifierActionLaunchGameButton)
                .AddArgument(Activation.Action, Activation.LaunchGame)
                .AddArgument(Activation.Uid, entry.Uid))
            .AddButton(new ToastButtonDismiss(SH.ServiceDailyNoteNotifierActionLaunchGameDismiss));

        if (options.IsReminderNotification)
        {
            builder.SetToastScenario(ToastScenario.Reminder);
        }

        if (notifyInfos.Count > 2)
        {
            builder.AddText(SH.ServiceDailyNoteNotifierMultiValueReached);

            // Desktop and Mobile started supporting adaptive toasts in API contract 3 (Anniversary Update)
            if (UniversalApiContract.IsPresent(WindowsVersion.Windows10AnniversaryUpdate))
            {
                AdaptiveGroup group = new();
                foreach (DailyNoteNotifyInfo info in notifyInfos)
                {
                    AdaptiveSubgroup subgroup = new()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveImage() { Source = info.AdaptiveIcon, HintRemoveMargin = true, },
                            new AdaptiveText() { Text = info.AdaptiveHint, HintAlign = AdaptiveTextAlign.Center,  },
                            new AdaptiveText() { Text = info.Title, HintAlign = AdaptiveTextAlign.Center, HintStyle = AdaptiveTextStyle.CaptionSubtle, },
                        },
                    };

                    group.Children.Add(subgroup);
                }

                builder.AddVisualChild(group);
            }
        }
        else
        {
            foreach (DailyNoteNotifyInfo info in notifyInfos)
            {
                builder.AddText(info.Hint);
            }
        }

        await taskContext.SwitchToMainThreadAsync();
        builder.Show(toast => toast.SuppressPopup = ShouldSuppressPopup(options));
    }

    private static void CheckNotifySuppressed(DailyNoteEntry entry, List<DailyNoteNotifyInfo> notifyInfos)
    {
        // Image limitation.
        // https://learn.microsoft.com/en-us/windows/apps/design/shell/tiles-and-notifications/send-local-toast?tabs=uwp#adding-images
        // NotifySuppressed judge
        ChcekResinNotifySuppressed(entry, notifyInfos);
        CheckHomeCoinNotifySuppressed(entry, notifyInfos);
        CheckDailyTaskNotifySuppressed(entry, notifyInfos);
        CheckTransformerNotifySuppressed(entry, notifyInfos);
        CheckExpeditionNotifySuppressed(entry, notifyInfos);
    }

    private static void ChcekResinNotifySuppressed(DailyNoteEntry entry, List<DailyNoteNotifyInfo> notifyInfos)
    {
        ArgumentNullException.ThrowIfNull(entry.DailyNote);
        if (entry.DailyNote.CurrentResin >= entry.ResinNotifyThreshold)
        {
            if (!entry.ResinNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierResin,
                    Web.HutaoEndpoints.StaticFile("ItemIcon", "UI_ItemIcon_210.png"),
                    $"{entry.DailyNote.CurrentResin}",
                    SH.ServiceDailyNoteNotifierResinCurrent.Format(entry.DailyNote.CurrentResin)));
                entry.ResinNotifySuppressed = true;
            }
        }
        else
        {
            entry.ResinNotifySuppressed = false;
        }
    }

    private static void CheckHomeCoinNotifySuppressed(DailyNoteEntry entry, List<DailyNoteNotifyInfo> notifyInfos)
    {
        ArgumentNullException.ThrowIfNull(entry.DailyNote);
        if (entry.DailyNote.CurrentHomeCoin >= entry.HomeCoinNotifyThreshold)
        {
            if (!entry.HomeCoinNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierHomeCoin,
                    Web.HutaoEndpoints.StaticFile("ItemIcon", "UI_ItemIcon_204.png"),
                    $"{entry.DailyNote.CurrentHomeCoin}",
                    SH.ServiceDailyNoteNotifierHomeCoinCurrent.Format(entry.DailyNote.CurrentHomeCoin)));
                entry.HomeCoinNotifySuppressed = true;
            }
        }
        else
        {
            entry.HomeCoinNotifySuppressed = false;
        }
    }

    private static void CheckDailyTaskNotifySuppressed(DailyNoteEntry entry, List<DailyNoteNotifyInfo> notifyInfos)
    {
        if (entry is { DailyTaskNotify: true, DailyNote.IsExtraTaskRewardReceived: false })
        {
            if (!entry.DailyTaskNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierDailyTask,
                    Web.HutaoEndpoints.StaticFile("Bg", "UI_MarkQuest_Events_Proce.png"),
                    SH.ServiceDailyNoteNotifierDailyTaskHint,
                    entry.DailyNote.ExtraTaskRewardDescription));
                entry.DailyTaskNotifySuppressed = true;
            }
        }
        else
        {
            entry.DailyTaskNotifySuppressed = false;
        }
    }

    private static void CheckTransformerNotifySuppressed(DailyNoteEntry entry, List<DailyNoteNotifyInfo> notifyInfos)
    {
        if (entry is { TransformerNotify: true, DailyNote.Transformer: { Obtained: true, RecoveryTime.Reached: true } })
        {
            if (!entry.TransformerNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierTransformer,
                    Web.HutaoEndpoints.StaticFile("ItemIcon", "UI_ItemIcon_220021.png"),
                    SH.ServiceDailyNoteNotifierTransformerAdaptiveHint,
                    SH.ServiceDailyNoteNotifierTransformerHint));
                entry.TransformerNotifySuppressed = true;
            }
        }
        else
        {
            entry.TransformerNotifySuppressed = false;
        }
    }

    private static void CheckExpeditionNotifySuppressed(DailyNoteEntry entry, List<DailyNoteNotifyInfo> notifyInfos)
    {
        ArgumentNullException.ThrowIfNull(entry.DailyNote);
        if (entry.ExpeditionNotify && entry.DailyNote.Expeditions.All(e => e.Status == ExpeditionStatus.Finished))
        {
            if (!entry.ExpeditionNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierExpedition,
                    Web.HutaoEndpoints.StaticFile("Bg", "UI_Icon_Intee_Explore_1.png"),
                    SH.ServiceDailyNoteNotifierExpeditionAdaptiveHint,
                    SH.ServiceDailyNoteNotifierExpeditionHint));
                entry.ExpeditionNotifySuppressed = true;
            }
        }
        else
        {
            entry.ExpeditionNotifySuppressed = false;
        }
    }

    private bool ShouldSuppressPopup(DailyNoteOptions options)
    {
        // Prevent notify when we are in game && silent mode.
        return options.IsSilentWhenPlayingGame && gameService.IsGameRunning();
    }
}