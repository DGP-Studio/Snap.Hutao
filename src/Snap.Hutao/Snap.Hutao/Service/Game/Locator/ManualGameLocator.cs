// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Picker;
using Windows.Storage.Pickers;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// 手动模式
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Transient)]
internal sealed partial class ManualGameLocator : IGameLocator
{
    private readonly ITaskContext taskContext;
    private readonly IPickerFactory pickerFactory;

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, string>> LocateGamePathAsync()
    {
        await taskContext.SwitchToMainThreadAsync();

        FileOpenPicker picker = pickerFactory.GetFileOpenPicker(
            PickerLocationId.Desktop,
            SH.ServiceGameLocatorFileOpenPickerCommitText,
            ".exe");

        (bool isPickerOk, ValueFile file) = await picker.TryPickSingleFileAsync().ConfigureAwait(false);

        if (isPickerOk)
        {
            string fileName = System.IO.Path.GetFileName(file);
            if (fileName is GameConstants.YuanShenFileName or GameConstants.GenshinImpactFileName)
            {
                return new(true, file);
            }
        }

        return new(false, default!);
    }
}