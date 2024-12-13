// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

// ReSharper disable InconsistentNaming
namespace Snap.Hutao.Win32.UI.Shell;

[Flags]
internal enum FILEOPENDIALOGOPTIONS : uint
{
    /// <summary>
    /// When saving a file, prompt before overwriting an existing file of the same name.
    /// This is a default value for the Save dialog.
    /// </summary>
    FOS_OVERWRITEPROMPT = 2U,

    /// <summary>
    /// In the Save dialog, only allow the user to choose a file that has one of the file name extensions specified through IFileDialog::SetFileTypes.
    /// </summary>
    FOS_STRICTFILETYPES = 4U,

    /// <summary>
    /// Don't change the current working directory.
    /// </summary>
    FOS_NOCHANGEDIR = 8U,

    /// <summary>
    /// Present an Open dialog that offers a choice of folders rather than files.
    /// </summary>
    FOS_PICKFOLDERS = 0x20U,

    /// <summary>
    /// Ensures that returned items are file system items (SFGAO_FILESYSTEM).
    /// Note that this does not apply to items returned by IFileDialog::GetCurrentSelection.
    /// </summary>
    FOS_FORCEFILESYSTEM = 0x40U,

    /// <summary>
    /// Enables the user to choose any item in the Shell namespace, not just those with SFGAO_STREAM or SFAGO_FILESYSTEM attributes.
    /// This flag cannot be combined with <see cref="FOS_FORCEFILESYSTEM"/>.
    /// </summary>
    FOS_ALLNONSTORAGEITEMS = 0x80U,

    /// <summary>
    /// Do not check for situations that would prevent an application from opening the selected file, such as sharing violations or access denied errors.
    /// </summary>
    FOS_NOVALIDATE = 0x100U,

    /// <summary>
    /// Enables the user to select multiple items in the open dialog. Note that when this flag is set, the IFileOpenDialog interface must be used to retrieve those items.
    /// </summary>
    FOS_ALLOWMULTISELECT = 0x200U,

    /// <summary>
    /// The item returned must be in an existing folder. This is a default value.
    /// </summary>
    FOS_PATHMUSTEXIST = 0x800U,

    /// <summary>
    /// The item returned must exist. This is a default value for the Open dialog.
    /// </summary>
    FOS_FILEMUSTEXIST = 0x1000U,

    /// <summary>
    /// Prompt for creation if the item returned in the open dialog does not exist.
    /// Note that this does not actually create the item.
    /// </summary>
    FOS_CREATEPROMPT = 0x2000U,

    /// <summary>
    /// In the case of a sharing violation when an application is opening a file, call the application back through OnShareViolation for guidance.
    /// This flag is overridden by FOS_NOVALIDATE.
    /// </summary>
    FOS_SHAREAWARE = 0x4000U,

    /// <summary>
    /// Do not return read-only items.
    /// This is a default value for the Save dialog.
    /// </summary>
    FOS_NOREADONLYRETURN = 0x8000U,

    /// <summary>
    /// Do not test whether creation of the item as specified in the Save dialog will be successful.
    /// If this flag is not set, the calling application must handle errors, such as denial of access, discovered when the item is created.
    /// </summary>
    FOS_NOTESTFILECREATE = 0x10000U,

    /// <summary>
    /// Hide the list of places from which the user has recently opened or saved items.
    /// This value is not supported as of Windows 7.
    /// </summary>
    FOS_HIDEMRUPLACES = 0x20000U,

    /// <summary>
    /// <para>
    /// Hide items shown by default in the view's navigation pane.
    /// This flag is often used in conjunction with the IFileDialog::AddPlace method, to hide standard locations and replace them with custom locations.
    /// </para>
    /// <para>
    /// <b>Windows 7 and later.</b>
    /// Hide all the standard namespace locations (such as Favorites, Libraries, Computer, and Network) shown in the navigation pane.
    /// </para>
    /// <para>
    /// <b>Windows Vista.</b>
    /// Hide the contents of the Favorite Links tree in the navigation pane.
    /// Note that the category itself is still displayed, but shown as empty.
    /// </para>
    /// </summary>
    FOS_HIDEPINNEDPLACES = 0x40000U,

    /// <summary>
    /// Shortcuts should not be treated as their target items.
    /// This allows an application to open a .lnk file rather than what that file is a shortcut to.
    /// </summary>
    FOS_NODEREFERENCELINKS = 0x100000U,

    /// <summary>
    /// The OK button will be disabled until the user navigates the view or edits the filename (if applicable).
    /// Note: Disabling of the OK button does not prevent the dialog from being submitted by the Enter key.
    /// </summary>
    FOS_OKBUTTONNEEDSINTERACTION = 0x200000U,

    /// <summary>
    /// Do not add the item being opened or saved to the recent documents list (SHAddToRecentDocs).
    /// </summary>
    FOS_DONTADDTORECENT = 0x2000000U,

    /// <summary>
    /// Include hidden and system items.
    /// </summary>
    FOS_FORCESHOWHIDDEN = 0x10000000U,

    /// <summary>
    /// Indicates to the Save As dialog box that it should open in expanded mode.
    /// Expanded mode is the mode that is set and unset by clicking the button in the lower-left corner of the Save As dialog box that switches between Browse Folders and Hide Folders when clicked.
    /// This value is not supported as of Windows 7.
    /// </summary>
    FOS_DEFAULTNOMINIMODE = 0x20000000U,

    /// <summary>
    /// Indicates to the Open dialog box that the preview pane should always be displayed.
    /// </summary>
    FOS_FORCEPREVIEWPANEON = 0x40000000U,

    /// <summary>
    /// Indicates that the caller is opening a file as a stream (BHID_Stream), so there is no need to download that file.
    /// </summary>
    FOS_SUPPORTSTREAMABLEITEMS = 0x80000000U,
}