// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;
using Snap.Hutao.Web.Response;
using Windows.Foundation.Metadata;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺通知器
/// </summary>
internal class DailyNoteNotifier
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly DailyNoteEntry entry;

    /// <summary>
    /// 构造一个新的实时便笺通知器
    /// </summary>
    /// <param name="scopeFactory">范围工厂</param>
    /// <param name="entry">实时便笺入口</param>
    public DailyNoteNotifier(IServiceScopeFactory scopeFactory, DailyNoteEntry entry)
    {
        this.scopeFactory = scopeFactory;
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

        // NotifySuppressed judge
        {
            if (entry.DailyNote.CurrentResin >= entry.ResinNotifyThreshold)
            {
                if (!entry.ResinNotifySuppressed)
                {
                    notifyInfos.Add(new(
                        "原粹树脂",
                        "ms-appx:///Resource/Icon/UI_ItemIcon_210_256.png",
                        $"{entry.DailyNote.CurrentResin}",
                        $"当前原粹树脂：{entry.DailyNote.CurrentResin}"));
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
                        "洞天宝钱",
                        "ms-appx:///Resource/Icon/UI_ItemIcon_204.png",
                        $"{entry.DailyNote.CurrentHomeCoin}",
                        $"当前洞天宝钱：{entry.DailyNote.CurrentHomeCoin}"));
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
                        "每日委托",
                        "ms-appx:///Resource/Icon/UI_MarkQuest_Events_Proce.png",
                        $"奖励待领取",
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
                        "参量质变仪",
                        "ms-appx:///Resource/Icon/UI_ItemIcon_220021.png",
                        $"准备完成",
                        "参量质变仪已准备完成"));
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
                        "探索派遣",
                        AvatarIconConverter.IconNameToUri("UI_AvatarIcon_Side_None.png").ToString(),
                        $"已完成",
                        "探索派遣已完成"));
                    entry.ExpeditionNotifySuppressed = true;
                }
            }
            else
            {
                entry.ExpeditionNotifySuppressed = false;
            }
        }

        if (notifyInfos.Count <= 0)
        {
            return;
        }

        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            BindingClient bindingClient = scope.ServiceProvider.GetRequiredService<BindingClient>();
            AuthClient authClient = scope.ServiceProvider.GetRequiredService<AuthClient>();

            Response<ActionTicketWrapper> actionTicketResponse = await authClient
                .GetActionTicketByStokenAsync("game_role", entry.User)
                .ConfigureAwait(false);

            string? attribution = "请求异常";
            if (actionTicketResponse.IsOk())
            {
                Response<ListWrapper<UserGameRole>> rolesResponse = await scope.ServiceProvider
                    .GetRequiredService<BindingClient>()
                    .GetUserGameRolesByActionTicketAsync(actionTicketResponse.Data.Ticket, entry.User)
                    .ConfigureAwait(false);

                if (rolesResponse.IsOk())
                {
                    List<UserGameRole> roles = rolesResponse.Data.List;
                    attribution = roles.SingleOrDefault(r => r.GameUid == entry.Uid)?.ToString() ?? "未知角色";
                }
            }

            ToastContentBuilder builder = new ToastContentBuilder()
                .AddHeader("DAILYNOTE", "实时便笺提醒", "DAILYNOTE")
                .AddAttributionText(attribution)
                .AddButton(new ToastButton().SetContent("开始游戏").AddArgument("Action", "LaunchGame").AddArgument("Uid", entry.Uid))
                .AddButton(new ToastButtonDismiss("我知道了"));

            if (appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteReminderNotify, SettingEntryHelper.FalseString).GetBoolean())
            {
                builder.SetToastScenario(ToastScenario.Reminder);
            }

            if (notifyInfos.Count > 2)
            {
                builder.AddText("多个提醒项达到设定值");

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

            await ThreadHelper.SwitchToMainThreadAsync();
            builder.Show();
        }
    }

    private struct NotifyInfo
    {
        public string Title;
        public string AdaptiveIcon;
        public string AdaptiveHint;
        public string Hint;

        public NotifyInfo(string title, string adaptiveIcon, string adaptiveHint, string hint)
        {
            Title = title;
            AdaptiveIcon = adaptiveIcon;
            AdaptiveHint = adaptiveHint;
            Hint = hint;
        }
    }
}