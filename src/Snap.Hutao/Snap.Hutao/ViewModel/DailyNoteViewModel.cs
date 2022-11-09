// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Control;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Service.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实时便笺视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class DailyNoteViewModel : ObservableObject, ISupportCancellation, IDisposable
{
    private readonly IUserService userService;

    private readonly List<NamedValue<int>> refreshTimes = new()
    {
        new("4 分钟 | 0.5 树脂", 240),
        new("8 分钟 | 1 树脂", 480),
        new("30 分钟 | 3.75 树脂", 1800),
        new("40 分钟 | 5 树脂", 2400),
        new("60 分钟 | 7.5 树脂", 3600),
    };

    private bool isReminderNotification;
    private NamedValue<int>? selectedRefreshTime;
    private ObservableCollection<UserAndRole>? userAndRoles;

    /// <summary>
    /// 构造一个新的实时便笺视图模型
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public DailyNoteViewModel(IUserService userService, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.userService = userService;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        TrackRoleCommand = asyncRelayCommandFactory.Create(TrackRoleAsync);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 刷新时间
    /// </summary>
    public List<NamedValue<int>> RefreshTimes { get => refreshTimes; }

    /// <summary>
    /// 选中的刷新时间
    /// </summary>
    public NamedValue<int>? SelectedRefreshTime { get => selectedRefreshTime; set => SetProperty(ref selectedRefreshTime, value); }

    /// <summary>
    /// 提醒式通知
    /// </summary>
    public bool IsReminderNotification { get => isReminderNotification; set => SetProperty(ref isReminderNotification, value); }

    /// <summary>
    /// 用户与角色集合
    /// </summary>
    public ObservableCollection<UserAndRole>? UserAndRoles { get => userAndRoles; set => userAndRoles = value; }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 跟踪角色命令
    /// </summary>
    public ICommand TrackRoleCommand { get; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    private async Task OpenUIAsync()
    {
        UserAndRoles = await userService.GetRoleCollectionAsync().ConfigureAwait(true);
    }

    private async Task TrackRoleAsync()
    {

    }
}