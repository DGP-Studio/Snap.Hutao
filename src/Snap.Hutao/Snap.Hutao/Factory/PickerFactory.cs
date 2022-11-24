// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.Abstraction;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Snap.Hutao.Factory;

/// <inheritdoc cref="IPickerFactory"/>
[Injection(InjectAs.Transient, typeof(IPickerFactory))]
internal class PickerFactory : IPickerFactory
{
    private const string AnyType = "*";

    private readonly MainWindow mainWindow;

    /// <summary>
    /// 构造一个新的文件选择器工厂
    /// </summary>
    /// <param name="mainWindow">主窗体的引用注入</param>
    public PickerFactory(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
    }

    /// <inheritdoc/>
    public FileOpenPicker GetFileOpenPicker(PickerLocationId location, string commitButton, params string[] fileTypes)
    {
        FileOpenPicker picker = GetInitializedPicker<FileOpenPicker>();

        picker.SuggestedStartLocation = location;
        picker.CommitButtonText = commitButton;

        foreach (string type in fileTypes)
        {
            picker.FileTypeFilter.Add(type);
        }

        picker.FileTypeFilter.Add(AnyType);

        return picker;
    }

    /// <inheritdoc/>
    public FileSavePicker GetFileSavePicker()
    {
        return GetInitializedPicker<FileSavePicker>();
    }

    /// <inheritdoc/>
    public FolderPicker GetFolderPicker()
    {
        return GetInitializedPicker<FolderPicker>();
    }

    private T GetInitializedPicker<T>()
        where T : new()
    {
        // Create a folder picker.
        T picker = new();

        IntPtr hWnd = WindowNative.GetWindowHandle(mainWindow);
        InitializeWithWindow.Initialize(picker, hWnd);

        return picker;
    }
}