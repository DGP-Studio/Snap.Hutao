// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;
using Snap.Hutao.Web.Response;
using Windows.Foundation.Metadata;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺通知器
/// </summary>
[HighQuality]
internal sealed class DailyNoteNotifier
{
    private readonly ITaskContext taskContext;
    private readonly IServiceProvider serviceProvider;
    private readonly DailyNoteEntry entry;

    /// <summary>
    /// 构造一个新的实时便笺通知器
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="entry">实时便笺入口</param>
    public DailyNoteNotifier(IServiceProvider serviceProvider, DailyNoteEntry entry)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        this.serviceProvider = serviceProvider;
        this.entry = entry;
    }

    /// <summary>
    /// 异步通知
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask NotifyAsync()
    {
        if (entry.DailyNote == null)
        {
            return;
        }

        List<NotifyInfo> notifyInfos = new();

        CheckNotifySuppressed(entry, notifyInfos);

        if (notifyInfos.Count <= 0)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            DailyNoteOptions options = scope.ServiceProvider.GetRequiredService<DailyNoteOptions>();
            BindingClient bindingClient = scope.ServiceProvider.GetRequiredService<BindingClient>();
            AuthClient authClient = scope.ServiceProvider.GetRequiredService<AuthClient>();

            string? attribution = SH.ServiceDailyNoteNotifierAttribution;

            Response<ListWrapper<UserGameRole>> rolesResponse = await scope.ServiceProvider
                .GetRequiredService<BindingClient>()
                .GetUserGameRolesOverseaAwareAsync(entry.User)
                .ConfigureAwait(false);

            if (rolesResponse.IsOk())
            {
                List<UserGameRole> roles = rolesResponse.Data.List;
                attribution = roles.SingleOrDefault(r => r.GameUid == entry.Uid)?.ToString() ?? "Unknown";
            }

            ToastContentBuilder builder = new ToastContentBuilder()
                .AddHeader("DAILYNOTE", SH.ServiceDailyNoteNotifierTitle, "DAILYNOTE")
                .AddAttributionText(attribution)
                .AddButton(new ToastButton()
                    .SetContent(SH.ServiceDailyNoteNotifierActionLaunchGameButton)
                    .AddArgument(Core.LifeCycle.Activation.Action, Core.LifeCycle.Activation.LaunchGame)
                    .AddArgument(Core.LifeCycle.Activation.Uid, entry.Uid))
                .AddButton(new ToastButtonDismiss(SH.ServiceDailyNoteNotifierActionLaunchGameDismiss));

            if (options.IsReminderNotification)
            {
                builder.SetToastScenario(ToastScenario.Reminder);
            }

            if (notifyInfos.Count > 2)
            {
                builder.AddText(SH.ServiceDailyNoteNotifierMultiValueReached);

                // Desktop and Mobile started supporting adaptive toasts in API contract 3 (Anniversary Update)
                if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3))
                {
                    AdaptiveGroup group = new();
                    foreach (NotifyInfo info in notifyInfos)
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
                foreach (NotifyInfo info in notifyInfos)
                {
                    builder.AddText(info.Hint);
                }
            }

            await taskContext.SwitchToMainThreadAsync();
            builder.Show(toast => toast.SuppressPopup = ShouldSuppressPopup(options));
        }
    }

    private static void CheckNotifySuppressed(DailyNoteEntry entry, List<NotifyInfo> notifyInfos)
    {
        // NotifySuppressed judge
        if (entry.DailyNote!.CurrentResin >= entry.ResinNotifyThreshold)
        {
            if (!entry.ResinNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierResin,
                    "ms-appx:///Resource/Icon/UI_ItemIcon_210_256.png",
                    $"{entry.DailyNote.CurrentResin}",
                    string.Format(SH.ServiceDailyNoteNotifierResinCurrent, entry.DailyNote.CurrentResin)));
                entry.ResinNotifySuppressed = true;
            }
        }
        else
        {
            entry.ResinNotifySuppressed = false;
        }

        if (entry.DailyNote.CurrentHomeCoin >= entry.HomeCoinNotifyThreshold)
        {
            if (!entry.HomeCoinNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierHomeCoin,
                    "ms-appx:///Resource/Icon/UI_ItemIcon_204.png",
                    $"{entry.DailyNote.CurrentHomeCoin}",
                    string.Format(SH.ServiceDailyNoteNotifierHomeCoinCurrent, entry.DailyNote.CurrentHomeCoin)));
                entry.HomeCoinNotifySuppressed = true;
            }
        }
        else
        {
            entry.HomeCoinNotifySuppressed = false;
        }

        if (entry.DailyTaskNotify && !entry.DailyNote.IsExtraTaskRewardReceived)
        {
            if (!entry.DailyTaskNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierDailyTask,
                    "ms-appx:///Resource/Icon/UI_MarkQuest_Events_Proce.png",
                    SH.ServiceDailyNoteNotifierDailyTaskHint,
                    entry.DailyNote.ExtraTaskRewardDescription));
                entry.DailyTaskNotifySuppressed = true;
            }
        }
        else
        {
            entry.DailyTaskNotifySuppressed = false;
        }

        if (entry.TransformerNotify && entry.DailyNote.Transformer.Obtained && entry.DailyNote.Transformer.RecoveryTime.Reached)
        {
            if (!entry.TransformerNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierTransformer,
                    "ms-appx:///Resource/Icon/UI_ItemIcon_220021.png",
                    SH.ServiceDailyNoteNotifierTransformerAdaptiveHint,
                    SH.ServiceDailyNoteNotifierTransformerHint));
                entry.TransformerNotifySuppressed = true;
            }
        }
        else
        {
            entry.TransformerNotifySuppressed = false;
        }

        if (entry.ExpeditionNotify && entry.DailyNote.Expeditions.All(e => e.Status == ExpeditionStatus.Finished))
        {
            if (!entry.ExpeditionNotifySuppressed)
            {
                notifyInfos.Add(new(
                    SH.ServiceDailyNoteNotifierExpedition,
                    Web.HutaoEndpoints.UIAvatarIconSideNone.ToString(), // TODO: embed this
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
        bool isGameRunning = serviceProvider.GetRequiredService<IGameService>().IsGameRunning();

        if (options.IsSilentWhenPlayingGame && isGameRunning)
        {
            // Prevent notify when we are in game && silent mode.
            return true;
        }
        else
        {
            return false;
        }
    }

    private readonly struct NotifyInfo
    {
        public readonly string Title;
        public readonly string AdaptiveIcon;
        public readonly string AdaptiveHint;
        public readonly string Hint;

        public NotifyInfo(string title, string adaptiveIcon, string adaptiveHint, string hint)
        {
            Title = title;
            AdaptiveIcon = adaptiveIcon;
            AdaptiveHint = adaptiveHint;
            Hint = hint;
        }
    }
}