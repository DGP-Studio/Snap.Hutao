// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.DailyNote;

/// <summary>
/// 简化的实时便笺视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal sealed class DailyNoteViewModelSlim : Abstraction.ViewModelSlim<View.Page.DailyNotePage>
{
    private List<DailyNoteEntry>? dailyNoteEntries;

    /// <summary>
    /// 构造一个新的简化的实时便笺视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public DailyNoteViewModelSlim(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    /// <summary>
    /// 实时便笺集合
    /// </summary>
    public List<DailyNoteEntry>? DailyNoteEntries { get => dailyNoteEntries; set => SetProperty(ref dailyNoteEntries, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        try
        {
            ITaskContext taskContext = ServiceProvider.GetRequiredService<ITaskContext>();

            await taskContext.SwitchToBackgroundAsync();
            _ = await ServiceProvider
                .GetRequiredService<IUserService>()
                .GetRoleCollectionAsync()
                .ConfigureAwait(false);
            ObservableCollection<DailyNoteEntry> entries = await ServiceProvider
                .GetRequiredService<IDailyNoteService>()
                .GetDailyNoteEntriesAsync()
                .ConfigureAwait(false);

            // We have to create a copy here,
            // to prevent page add/remove failure.
            List<DailyNoteEntry> entryList = entries.ToList();

            await taskContext.SwitchToMainThreadAsync();
            DailyNoteEntries = entryList;
            IsInitialized = true;
        }
        catch (Core.ExceptionService.UserdataCorruptedException ex)
        {
            ServiceProvider.GetRequiredService<IInfoBarService>().Error(ex);
            return;
        }
    }
}