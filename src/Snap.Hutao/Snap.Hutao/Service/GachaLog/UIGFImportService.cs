// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录导入服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IUIGFImportService))]
internal sealed partial class UIGFImportService : IUIGFImportService
{
    private readonly ILogger<UIGFImportService> logger;
    private readonly MetadataOptions metadataOptions;
    private readonly IGachaLogDbService gachaLogDbService;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<GachaArchive> ImportAsync(GachaLogServiceMetadataContext context, UIGF uigf, ObservableCollection<GachaArchive> archives)
    {
        await taskContext.SwitchToBackgroundAsync();

        if (!uigf.IsCurrentVersionSupported(out UIGFVersion version))
        {
            ThrowHelper.InvalidOperation(SH.ServiceUIGFImportUnsupportedVersion);
        }

        // v2.3+ support any locale
        // v2.2 only support matched locale
        // v2.1 only support CHS
        if (version is UIGFVersion.Major2Minor2OrLower)
        {
            if (!metadataOptions.IsCurrentLocale(uigf.Info.Language))
            {
                string message = SH.ServiceGachaUIGFImportLanguageNotMatch.Format(uigf.Info.Language, metadataOptions.LanguageCode);
                ThrowHelper.InvalidOperation(message);
            }

            if (!uigf.IsMajor2Minor2OrLowerListValid(out long id))
            {
                string message = SH.ServiceGachaLogUIGFImportItemInvalidFormat.Format(id);
                ThrowHelper.InvalidOperation(message);
            }
        }

        if (version is UIGFVersion.Major2Minor3OrHigher)
        {
            if (!uigf.IsMajor2Minor3OrHigherListValid(out long id))
            {
                string message = SH.ServiceGachaLogUIGFImportItemInvalidFormat.Format(id);
                ThrowHelper.InvalidOperation(message);
            }
        }

        GachaArchiveOperation.GetOrAdd(gachaLogDbService, taskContext, uigf.Info.Uid, archives, out GachaArchive? archive);
        Guid archiveId = archive.InnerId;

        List<GachaItem> fullItems = new();
        foreach (GachaConfigType queryType in GachaLog.QueryTypes)
        {
            long trimId = gachaLogDbService.GetOldestGachaItemIdByArchiveIdAndQueryType(archiveId, queryType);
            logger.LogInformation("Last Id to trim with: [{Id}]", trimId);

            List<GachaItem> currentTypedList = version switch
            {
                UIGFVersion.Major2Minor3OrHigher => uigf.List
                    .Where(i => i.UIGFGachaType == queryType && i.Id < trimId)
                    .OrderByDescending(i => i.Id)
                    .Select(i => GachaItem.From(archiveId, i))
                    .ToList(),
                UIGFVersion.Major2Minor2OrLower => uigf.List
                    .Where(i => i.UIGFGachaType == queryType && i.Id < trimId)
                    .OrderByDescending(i => i.Id)
                    .Select(i => GachaItem.From(archiveId, i, context.GetItemId(i)))
                    .ToList(),
                _ => throw Must.NeverHappen(),
            };

            ThrowIfContainsInvalidItem(currentTypedList);
            fullItems.AddRange(currentTypedList);
        }

        await gachaLogDbService.AddGachaItemsAsync(fullItems).ConfigureAwait(false);
        return archive;
    }

    private static void ThrowIfContainsInvalidItem(List<GachaItem> currentTypeToAdd)
    {
        // 越早的记录手工导入的可能性越高
        // 错误率相对来说会更高
        // 因此从尾部开始查找
        if (currentTypeToAdd.LastOrDefault(item => item.ItemId is 0U) is { } item)
        {
            ThrowHelper.InvalidOperation(SH.ServiceGachaLogUIGFImportItemInvalidFormat.Format(item.Id));
        }
    }
}