// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.UI.Shell.Common;

[SuppressMessage("", "SA1307")]
internal struct COMDLG_FILTERSPEC
{
    public PCWSTR pszName;
    public PCWSTR pszSpec;
}