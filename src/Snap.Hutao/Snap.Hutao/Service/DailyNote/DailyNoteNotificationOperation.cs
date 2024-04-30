// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.DailyNote.NotifySuppression;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
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
    private readonly IGameServiceFacade gameService;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly DailyNoteOptions options;
    private readonly RuntimeOptions runtimeOptions;

    public async ValueTask SendAsync(DailyNoteEntry entry)
    {
        if (!runtimeOptions.IsToastAvailable)
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

        string? attribution = SH.ServiceDailyNoteNotifierAttribution;

        if (entry.UserGameRole is not null)
        {
            attribution = entry.UserGameRole.ToString();
        }
        else
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
                            // new AdaptiveImage() { Source = info.AdaptiveIcon, HintRemoveMargin = true, },
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

    private bool ShouldSuppressPopup(DailyNoteOptions options)
    {
        // Prevent notify when we are in game && silent mode.
        return options.IsSilentWhenPlayingGame && gameService.IsGameRunning();
    }
}