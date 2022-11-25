// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// ICoreWebView2Interop
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("912b34a7-d10b-49c4-af18-7cb7e604e01a")]
public interface ICoreWebView2Interop
{
    /// <summary>
    /// Add the provided host object to script running in the WebView with the specified name.
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="obj">对象</param>
    /// <returns>结果</returns>
    HRESULT AddHostObjectToScript([In] string name, [In] ref object obj);
}