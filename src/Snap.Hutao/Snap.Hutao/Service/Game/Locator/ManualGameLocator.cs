﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Abstraction;
using Windows.Storage.Pickers;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// 手动模式
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGameLocator))]
internal sealed partial class ManualGameLocator : IGameLocator
{
    private readonly ITaskContext taskContext;
    private readonly IPickerFactory pickerFactory;

    /// <inheritdoc/>
    public string Name { get => nameof(ManualGameLocator); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> LocateGamePathAsync(ValueResult<bool, bool> locateConfig)
    {
        await taskContext.SwitchToMainThreadAsync();

        FileOpenPicker picker = pickerFactory.GetFileOpenPicker(
                PickerLocationId.Desktop,
                SH.ServiceGameLocatorFileOpenPickerCommitText,
                locateConfig.Value ? ".dll" : ".exe");

        (bool isPickerOk, ValueFile file) = await picker.TryPickSingleFileAsync().ConfigureAwait(false);

        if (isPickerOk)
        {
            string fileName = System.IO.Path.GetFileName(file);
            if (locateConfig.Value)
            {
                return new(true, file);
            }

            if ((fileName == GameConstants.YuanShenFileName || fileName == GameConstants.GenshinImpactFileName) && !locateConfig.IsOk)
            {
                return new(true, file);
            }

            if (locateConfig.IsOk && fileName == GameConstants.StarRailFileName)
            {
                return new(true, file);
            }
        }

        return new(false, null!);
    }
}