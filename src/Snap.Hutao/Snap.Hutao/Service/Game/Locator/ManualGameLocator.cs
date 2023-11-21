// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Picker;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// 手动模式
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Transient)]
internal sealed partial class ManualGameLocator : IGameLocator
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;

    /// <inheritdoc/>
    public ValueTask<ValueResult<bool, string>> LocateGamePathAsync()
    {
        (bool isPickerOk, ValueFile file) = fileSystemPickerInteraction.PickFile(
            SH.ServiceGameLocatorFileOpenPickerCommitText,
            [(SH.ServiceGameLocatorPickerFilterText, $"{GameConstants.YuanShenFileName};{GameConstants.GenshinImpactFileName}")]);

        if (isPickerOk)
        {
            string fileName = System.IO.Path.GetFileName(file);
            if (fileName is GameConstants.YuanShenFileName or GameConstants.GenshinImpactFileName)
            {
                return ValueTask.FromResult<ValueResult<bool, string>>(new(true, file));
            }
        }

        return ValueTask.FromResult<ValueResult<bool, string>>(new(false, default!));
    }
}