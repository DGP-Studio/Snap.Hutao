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

        if (!metadataOptions.IsCurrentLocale(uigf.Info.Language))
        {
            string message = SH.ServiceGachaUIGFImportLanguageNotMatch.Format(uigf.Info.Language, metadataOptions.LanguageCode);
            ThrowHelper.InvalidOperation(message, null);
        }

        GachaArchiveOperation.GetOrAdd(gachaLogDbService, taskContext, uigf.Info.Uid, archives, out GachaArchive? archive);
        Guid archiveId = archive.InnerId;

        _ = uigf.IsCurrentVersionSupported(out UIGFVersion version);

        foreach (GachaConfigType queryType in GachaLog.QueryTypes)
        {
            long trimId = gachaLogDbService.GetOldestGachaItemIdByArchiveIdAndQueryType(archiveId, queryType);
            logger.LogInformation("Last Id to trim with: [{Id}]", trimId);

            List<GachaItem> currentTypeToAdd = version switch
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
                _ => throw ThrowHelper.InvalidOperation(SH.ServiceUIGFImportUnsupportedVersion),
            };

            // 越早的记录手工导入的可能性越高
            // 错误率相对来说会更高
            // 因此从尾部开始查找
            if (currentTypeToAdd.LastOrDefault(item => item.ItemId is 0U) is { } item)
            {
                ThrowHelper.InvalidOperation(SH.ServiceGachaLogUIGFImportItemInvalidFormat.Format(item.Id));
            }

            await gachaLogDbService.AddGachaItemsAsync(currentTypeToAdd).ConfigureAwait(false);
        }

        return archive;
    }
}