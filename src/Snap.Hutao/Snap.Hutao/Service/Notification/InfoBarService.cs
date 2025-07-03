// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Notification;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IInfoBarService))]
internal sealed partial class InfoBarService : IInfoBarService
{
    private readonly Lock syncRoot = new();
    private readonly ITaskContext taskContext;

    [field: MaybeNull]
    public ObservableCollection<InfoBarOptions> Collection
    {
        get
        {
            if (field is null)
            {
                lock (syncRoot)
                {
                    field ??= [];
                }
            }

            return field;
        }
    }

    public void PrepareInfoBarAndShow(Action<IInfoBarOptionsBuilder> configure)
    {
        PrivatePrepareInfoBarAndShowAsync(configure).SafeForget();
    }

    private async ValueTask PrivatePrepareInfoBarAndShowAsync(Action<IInfoBarOptionsBuilder> configure)
    {
        IInfoBarOptionsBuilder builder = new InfoBarOptionsBuilder().Configure(configure);

        await taskContext.SwitchToMainThreadAsync();

        try
        {
            Collection.Insert(0, builder.Options);
        }
        catch
        {
            // Ignore
        }
    }
}