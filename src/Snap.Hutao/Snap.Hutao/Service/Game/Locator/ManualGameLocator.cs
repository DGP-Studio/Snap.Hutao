// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading;
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
        return LocateInternalAsync("YuanShen.exe");
    }

    /// <inheritdoc/>
    public Task<ValueResult<bool, string>> LocateLauncherPathAsync()
    {
        return LocateInternalAsync("launcher.exe");
    }

    private async Task<ValueResult<bool, string>> LocateInternalAsync(string fileName)
    {
        FileOpenPicker picker = pickerFactory.GetFileOpenPicker();
        picker.FileTypeFilter.Add(".exe");
        picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

        if (await picker.PickSingleFileAsync() is StorageFile file)
        {
            string path = file.Path;
            if (path.Contains(fileName))
            {
                return new(true, path);
            }
        }

        return new(false, null!);
    }
}