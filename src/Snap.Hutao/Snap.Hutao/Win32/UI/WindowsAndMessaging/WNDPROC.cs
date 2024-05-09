// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

[UnmanagedFunctionPointer(CallingConvention.Winapi)]
internal delegate LRESULT WNDPROC(HWND param0, uint param1, WPARAM param2, LPARAM param3);