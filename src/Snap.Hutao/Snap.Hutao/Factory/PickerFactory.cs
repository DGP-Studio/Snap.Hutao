// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Factory.Abstraction;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Snap.Hutao.Factory;

/// <inheritdoc cref="IPickerFactory"/>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IPickerFactory))]
internal sealed partial class PickerFactory : IPickerFactory
{
    private const string AnyType = "*";

    private readonly ICurrentWindowReference currentWindow;

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

        // below Windows 11
        if (!UniversalApiContract.IsPresent(WindowsVersion.Windows11))
        {
            // https://github.com/microsoft/WindowsAppSDK/issues/2931
            picker.FileTypeFilter.Add(AnyType);
        }

        return picker;
    }

    /// <inheritdoc/>
    public FileSavePicker GetFileSavePicker(PickerLocationId location, string fileName, string commitButton, IDictionary<string, IList<string>> fileTypes)
    {
        FileSavePicker picker = GetInitializedPicker<FileSavePicker>();

        picker.SuggestedStartLocation = location;
        picker.SuggestedFileName = fileName;
        picker.CommitButtonText = commitButton;

        foreach (KeyValuePair<string, IList<string>> kvp in fileTypes)
        {
            picker.FileTypeChoices.Add(kvp);
        }

        return picker;
    }

    /// <inheritdoc/>
    public FolderPicker GetFolderPicker()
    {
        FolderPicker picker = GetInitializedPicker<FolderPicker>();

        // below Windows 11
        if (!UniversalApiContract.IsPresent(WindowsVersion.Windows11))
        {
            // https://github.com/microsoft/WindowsAppSDK/issues/2931
            picker.FileTypeFilter.Add(AnyType);
        }

        return picker;
    }

    private T GetInitializedPicker<T>()
        where T : new()
    {
        // Create a folder picker.
        T picker = new();
        if (currentWindow.Window is IWindowOptionsSource optionsSource)
        {
            InitializeWithWindow.Initialize(picker, optionsSource.WindowOptions.Hwnd);
        }

        return picker;
    }
}