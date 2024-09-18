// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.IO;

namespace Snap.Hutao.Core.IO;

internal readonly struct TempFile : IDisposable
{
    public readonly string Path;

    private TempFile(bool delete)
    {
        try
        {
            Path = System.IO.Path.GetTempFileName();
        }
        catch (UnauthorizedAccessException ex)
        {
            HutaoException.Throw(SH.CoreIOTempFileCreateFail, ex);
        }

        if (delete)
        {
            File.Delete(Path);
        }
    }

    public static TempFile? CopyFrom(string file)
    {
        TempFile temporaryFile = new(false);
        try
        {
            File.Copy(file, temporaryFile.Path, true);
            return temporaryFile;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void Dispose()
    {
        try
        {
            File.Delete(Path);
        }
        catch (IOException)
        {
        }
    }
}