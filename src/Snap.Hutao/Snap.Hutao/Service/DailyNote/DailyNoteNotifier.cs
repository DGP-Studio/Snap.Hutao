// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺通知器
/// </summary>
internal class DailyNoteNotifier
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly BindingClient bindingClient;
    private readonly DailyNoteEntry entry;

    /// <summary>
    /// 构造一个新的实时便笺通知器
    /// </summary>
    /// <param name="scopeFactory">范围工厂</param>
    /// <param name="bindingClient">绑定客户端</param>
    /// <param name="entry">实时便笺入口</param>
    public DailyNoteNotifier(IServiceScopeFactory scopeFactory, BindingClient bindingClient, DailyNoteEntry entry)
    {
        this.scopeFactory = scopeFactory;
        this.bindingClient = bindingClient;
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

        List<string> hints = new();

        // NotifySuppressed judge
        {
            if (entry.DailyNote.CurrentResin >= entry.ResinNotifyThreshold)
            {
                if (!entry.ResinNotifySuppressed)
                {
                    hints.Add($"当前原粹树脂：{entry.DailyNote.CurrentResin}");
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
                    hints.Add($"当前洞天宝钱：{entry.DailyNote.CurrentHomeCoin}");
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
                    hints.Add(entry.DailyNote.ExtraTaskRewardDescription);
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
                    hints.Add("参量质变仪已准备完成");
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
                    hints.Add("探索派遣已完成");
                    entry.ExpeditionNotifySuppressed = true;
                }
            }
            else
            {
                entry.ExpeditionNotifySuppressed = false;
            }
        }

        if (hints.Count <= 0)
        {
            return;
        }

        List<UserGameRole> roles = await bindingClient.GetUserGameRolesByCookieAsync(entry.User).ConfigureAwait(false);
        string attribution = roles.SingleOrDefault(r => r.GameUid == entry.Uid)?.ToString() ?? "未知角色";

        ToastContentBuilder builder = new ToastContentBuilder()
            .AddHeader("DAILYNOTE", "实时便笺提醒", "DAILYNOTE")
            .AddAttributionText(attribution)
            .AddButton(new ToastButton().SetContent("开始游戏").AddArgument("Action", "LaunchGame").AddArgument("Uid", entry.Uid))
            .AddButton(new ToastButtonDismiss("我知道了"));

        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteReminderNotify, SettingEntryHelper.FalseString).GetBoolean())
            {
                builder.SetToastScenario(ToastScenario.Reminder);
            }
        }

        if (hints.Count > 2)
        {
            builder.AddText("多个提醒项达到设定值");
        }
        else
        {
            foreach (string hint in hints)
            {
                builder.AddText(hint);
            }
        }

        await ThreadHelper.SwitchToMainThreadAsync();
        builder.Show();
    }
}