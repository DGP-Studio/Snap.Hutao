// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Threading;

namespace Snap.Hutao.Win32;

internal struct HutaoNativeProcessStartInfo
{
    public PCWSTR ApplicationName;
    public PCWSTR CommandLine;
    public BOOL InheritHandles;
    public PROCESS_CREATION_FLAGS CreationFlags;
    public PCWSTR CurrentDirectory;
}