// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using Snap.Hutao.Win32.UI.Shell;

namespace Snap.Hutao.Core.IO;

internal static class FileSystem
{
    [field: MaybeNull]
    private static HutaoNativeFileSystem Native
    {
        get => LazyInitializer.EnsureInitialized(ref field, HutaoNative.Instance.MakeFileSystem);
    }

    public static void RenameItem(string filePath, string newName)
    {
        Native.RenameItem(filePath, newName);
    }

    public static void RenameItem(string filePath, string newName, FILEOPERATION_FLAGS options)
    {
        Native.RenameItemWithOptions(filePath, newName, options);
    }

    public static void MoveItem(string oldPath, string newFolder)
    {
        Native.MoveItem(oldPath, newFolder);
    }

    public static void MoveItem(string oldPath, string newFolder, FILEOPERATION_FLAGS options)
    {
        Native.MoveItemWithOptions(oldPath, newFolder, options);
    }

    public static void MoveItem(string oldPath, string newFolder, string name)
    {
        Native.MoveItemWithName(oldPath, newFolder, name);
    }

    public static void MoveItem(string oldPath, string newFolder, string name, FILEOPERATION_FLAGS options)
    {
        Native.MoveItemWithNameAndOptions(oldPath, newFolder, name, options);
    }

    public static void CopyItem(string oldPath, string newFolder)
    {
        Native.CopyItem(oldPath, newFolder);
    }

    public static void CopyItem(string oldPath, string newFolder, FILEOPERATION_FLAGS options)
    {
        Native.CopyItemWithOptions(oldPath, newFolder, options);
    }

    public static void CopyItem(string oldPath, string newFolder, string name)
    {
        Native.CopyItemWithName(oldPath, newFolder, name);
    }

    public static void CopyItem(string oldPath, string newFolder, string name, FILEOPERATION_FLAGS options)
    {
        Native.CopyItemWithNameAndOptions(oldPath, newFolder, name, options);
    }

    public static void DeleteItem(string path)
    {
        Native.DeleteItem(path);
    }

    public static void DeleteItem(string path, FILEOPERATION_FLAGS options)
    {
        Native.DeleteItemWithOptions(path, options);
    }
}