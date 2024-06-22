// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Notification;

/// <inheritdoc/>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(IInfoBarService))]
internal sealed class InfoBarService : IInfoBarService
{
    private readonly ILogger<InfoBarService> logger;
    private readonly ITaskContext taskContext;

    private ObservableCollection<InfoBarOptions>? collection;

    public InfoBarService(IServiceProvider serviceProvider)
    {
        logger = serviceProvider.GetRequiredService<ILogger<InfoBarService>>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    /// <inheritdoc/>
    public ObservableCollection<InfoBarOptions> Collection
    {
        get => collection ??= [];
    }

    public void PrepareInfoBarAndShow(Action<IInfoBarOptionsBuilder> configure)
    {
        if (collection is null)
        {
            return;
        }

        PrepareInfoBarAndShowCoreAsync(configure).SafeForget(logger);
    }

    private async ValueTask PrepareInfoBarAndShowCoreAsync(Action<IInfoBarOptionsBuilder> configure)
    {
        IInfoBarOptionsBuilder builder = new InfoBarOptionsBuilder().Configure(configure);

        await taskContext.SwitchToMainThreadAsync();

        ArgumentNullException.ThrowIfNull(collection);
        collection.Add(builder.Options);
    }
}