// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Page;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.DailyNote;

/// <summary>
/// 简化的实时便笺视图模型
/// </summary>
[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class DailyNoteViewModelSlim : Abstraction.ViewModelSlim<DailyNotePage>
{
    private readonly ITaskContext taskContext;
    private readonly IInfoBarService infoBarService;
    private readonly IMetadataService metadataService;
    private readonly IDailyNoteService dailyNoteService;

    private List<DailyNoteEntry>? dailyNoteEntries;

    /// <summary>
    /// 实时便笺集合
    /// </summary>
    public List<DailyNoteEntry>? DailyNoteEntries { get => dailyNoteEntries; set => SetProperty(ref dailyNoteEntries, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            try
            {
                await taskContext.SwitchToBackgroundAsync();
                ObservableCollection<DailyNoteEntry> entries = await dailyNoteService
                    .GetDailyNoteEntryCollectionAsync()
                    .ConfigureAwait(false);

                // 此处使用浅拷贝的列表以避免当导航到实时便笺页面后
                // 由于主页尚未卸载，添加或删除便笺可能会崩溃的问题
                List<DailyNoteEntry> entryList = [.. entries];

                await taskContext.SwitchToMainThreadAsync();
                DailyNoteEntries = entryList;
                IsInitialized = true;
            }
            catch (HutaoException ex)
            {
                infoBarService.Error(ex);
                return;
            }
        }
    }
}