// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Picker;

namespace Snap.Hutao.Service.Game.Locator;

[Service(ServiceLifetime.Transient, typeof(IGameLocator), Key = GameLocationSourceKind.Manual)]
internal sealed partial class ManualGameLocator : IGameLocator
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;

    [GeneratedConstructor]
    public partial ManualGameLocator(IServiceProvider serviceProvider);

    public ValueTask<ValueResult<bool, string>> LocateSingleGamePathAsync()
    {
        (bool isPickerOk, ValueFile file) = fileSystemPickerInteraction.PickFile(
            SH.ServiceGameLocatorFileOpenPickerCommitText,
            SH.ServiceGameLocatorPickerFilterText,
            $"{GameConstants.YuanShenFileName};{GameConstants.GenshinImpactFileName}");

        if (isPickerOk)
        {
            string fileName = System.IO.Path.GetFileName(file);
            if (fileName.ToUpperInvariant() is GameConstants.YuanShenFileNameUpper or GameConstants.GenshinImpactFileNameUpper)
            {
                return ValueTask.FromResult<ValueResult<bool, string>>(new(true, file));
            }
        }

        return ValueTask.FromResult<ValueResult<bool, string>>(new(false, default!));
    }
}