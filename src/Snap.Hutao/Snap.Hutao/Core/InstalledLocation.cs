// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Snap.Hutao.Core;

internal static class InstalledLocation
{
    public static string GetAbsolutePath(string relativePath)
    {
        return Path.Combine(Package.Current.InstalledLocation.Path, relativePath);
    }

    public static void CopyFileFromApplicationUri(string url, string path)
    {
        CopyApplicationUriFileCoreAsync(url, path).GetAwaiter().GetResult();

        static async Task CopyApplicationUriFileCoreAsync(string url, string path)
        {
            await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(url.ToUri());
            using (Stream outputStream = (await file.OpenReadAsync()).AsStreamForRead())
            {
                if (File.Exists(path))
                {
                    try
                    {
                        FileInfo fileInfo = new(path);
                        FileSecurity fileSecurity = fileInfo.GetAccessControl();
                        SecurityIdentifier? user = WindowsIdentity.GetCurrent().User;

                        if (user is not null)
                        {
                            fileSecurity.AddAccessRule(new(user, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                            fileInfo.SetAccessControl(fileSecurity);
                        }
                    }
                    catch
                    {
                        // Ignore
                    }
                }

                using (FileStream inputStream = File.Create(path))
                {
                    await outputStream.CopyToAsync(inputStream).ConfigureAwait(false);
                }
            }
        }
    }
}