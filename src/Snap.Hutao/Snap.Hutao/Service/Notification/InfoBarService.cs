// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Notification;

[Service(ServiceLifetime.Singleton, typeof(IInfoBarService))]
internal sealed partial class InfoBarService : IInfoBarService, IRecipient<InfoBarMessage>
{
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial InfoBarService(IServiceProvider serviceProvider);

    [field: MaybeNull]
    public ObservableCollection<InfoBarOptions> Collection { get => LazyInitializer.EnsureInitialized(ref field, () => []); }

    public void Receive(InfoBarMessage message)
    {
        PrivateShowAsync(InfoBarOptions.Create(message)).SafeForget();
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