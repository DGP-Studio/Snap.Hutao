// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IUIGFImportService))]
internal sealed partial class UIGFImportService : IUIGFImportService
{
    private readonly IGachaLogDbService gachaLogDbService;
    private readonly ILogger<UIGFImportService> logger;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;

    public async ValueTask ImportAsync(GachaLogServiceMetadataContext context, UIGF uigf, AdvancedDbCollectionView<GachaArchive> archives)
    {
        await taskContext.SwitchToBackgroundAsync();

        if (!uigf.IsCurrentVersionSupported(out UIGFVersion version))
        {
            HutaoException.InvalidOperation(SH.ServiceUIGFImportUnsupportedVersion);
        }

        // v2.3+ supports any locale
        // v2.2 only supports matched locale
        // v2.1 only supports CHS
        if (version is UIGFVersion.Major2Minor2OrLower)
        {
            if (!cultureOptions.LanguageCodeFitsCurrentLocale(uigf.Info.Language))
            {
                string message = SH.FormatServiceGachaUIGFImportLanguageNotMatch(uigf.Info.Language, cultureOptions.LanguageCode);
                HutaoException.InvalidOperation(message);
            }

            if (!uigf.IsMajor2Minor2OrLowerListValid(out long id))
            {
                string message = SH.FormatServiceGachaLogUIGFImportItemInvalidFormat(id);
                HutaoException.InvalidOperation(message);
            }
        }

        if (version is UIGFVersion.Major2Minor3OrHigher)
        {
            if (!uigf.IsMajor2Minor3OrHigherListValid(out long id))
            {
                string message = SH.FormatServiceGachaLogUIGFImportItemInvalidFormat(id);
                HutaoException.InvalidOperation(message);
            }
        }

        GachaArchiveOperation.GetOrAdd(gachaLogDbService, taskContext, uigf.Info.Uid, archives, out GachaArchive? archive);
        Guid archiveId = archive.InnerId;

        List<GachaItem> fullItems = [];
        foreach (GachaType queryType in GachaLog.QueryTypes)
        {
            long trimId = gachaLogDbService.GetOldestGachaItemIdByArchiveIdAndQueryType(archiveId, queryType);
            logger.LogInformation("Last Id to trim with: [{Id}]", trimId);

            List<GachaItem> currentTypedList = version switch
            {
                UIGFVersion.Major2Minor3OrHigher => uigf.List
                    .Where(item => item.UIGFGachaType == queryType && item.Id < trimId)
                    .OrderByDescending(item => item.Id)
                    .Select(item => GachaItem.From(archiveId, item))
                    .ToList(),
                UIGFVersion.Major2Minor2OrLower => uigf.List
                    .Where(item => item.UIGFGachaType == queryType && item.Id < trimId)
                    .OrderByDescending(item => item.Id)
                    .Select(item => GachaItem.From(archiveId, item, context.GetItemId(item)))
                    .ToList(),
                _ => throw HutaoException.NotSupported(),
            };

            ThrowIfContainsInvalidItem(currentTypedList);
            fullItems.AddRange(currentTypedList);
        }

        gachaLogDbService.AddGachaItemRange(fullItems);
        archives.MoveCurrentTo(archive);
    }

    private static void ThrowIfContainsInvalidItem(List<GachaItem> list)
    {
        // 越早的记录手工导入的可能性越高
        // 错误率相对来说会更高
        // 因此从尾部开始查找
        if (list.LastOrDefault(item => item.ItemId is 0U) is { } item)
        {
            HutaoException.InvalidOperation(SH.FormatServiceGachaLogUIGFImportItemInvalidFormat(item.Id));
        }
    }
}