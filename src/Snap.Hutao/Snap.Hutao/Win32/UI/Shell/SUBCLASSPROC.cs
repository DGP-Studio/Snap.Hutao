// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.Shell;

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
internal delegate LRESULT SUBCLASSPROC(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData);