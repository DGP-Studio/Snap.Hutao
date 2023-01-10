// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Abstraction;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Snap.Hutao.Service.Game.Locator;

/// <summary>
/// 手动模式
/// </summary>
[Injection(InjectAs.Transient, typeof(IGameLocator))]
internal class ManualGameLocator : IGameLocator
{
    private readonly IPickerFactory pickerFactory;

    /// <summary>
    /// 构造一个新的手动模式提供器
    /// </summary>
    /// <param name="pickerFactory">选择器工厂</param>
    public ManualGameLocator(IPickerFactory pickerFactory)
    {
        this.pickerFactory = pickerFactory;
    }

    /// <inheritdoc/>
    public string Name { get => nameof(ManualGameLocator); }

    /// <inheritdoc/>
    public Task<ValueResult<bool, string>> LocateGamePathAsync()
    {
        List<string> filenames = new(2) { "YuanShen.exe", "GenshinImpact.exe", };
        return LocateInternalAsync(filenames);
    }

    private async Task<ValueResult<bool, string>> LocateInternalAsync(List<string> fileNames)
    {
        FileOpenPicker picker = pickerFactory.GetFileOpenPicker(PickerLocationId.Desktop, "选择游戏本体", ".exe");
        (bool isPickerOk, FilePath file) = await picker.TryPickSingleFileAsync().ConfigureAwait(false);

        if (isPickerOk)
        {
            string fileName = System.IO.Path.GetFileName(file);
            if (fileNames.Contains(fileName))
            {
                return new(true, file);
            }
        }

        return new(false, null!);
    }
}