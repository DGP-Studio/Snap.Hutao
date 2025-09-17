// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Notification;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(IInfoBarService))]
internal sealed partial class InfoBarService : IInfoBarService, IRecipient<InfoBarMessage>
{
    private readonly ITaskContext taskContext;

    [field: MaybeNull]
    public ObservableCollection<InfoBarOptions> Collection { get => LazyInitializer.EnsureInitialized(ref field, () => []); }

    public void Receive(InfoBarMessage message)
    {
        PrivateShowAsync(InfoBarOptions.Create(message)).SafeForget();
    }

    public void PrepareInfoBarAndShow(Action<IInfoBarOptionsBuilder> configure)
    {
        PrivatePrepareInfoBarAndShowAsync(configure).SafeForget();
    }

    private ValueTask PrivatePrepareInfoBarAndShowAsync(Action<IInfoBarOptionsBuilder> configure)
    {
        return PrivateShowAsync(new InfoBarOptionsBuilder().Configure(configure).Options);
    }

    private async ValueTask PrivateShowAsync(InfoBarOptions infoBarOptions)
    {
        await taskContext.SwitchToMainThreadAsync();

        try
        {
            Collection.Insert(0, infoBarOptions);
        }
        catch
        {
            // Ignore
        }
    }
}