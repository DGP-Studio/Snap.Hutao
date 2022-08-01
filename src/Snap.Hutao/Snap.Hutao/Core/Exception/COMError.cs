// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Exception;

/// <summary>
/// Error codes used by COM-based APIs.
/// </summary>
public enum COMError : uint
{
    /// <summary>
    /// The component cannot be found.
    /// </summary>
    WINCODEC_ERR_COMPONENTNOTFOUND = 0x88982F50,
}
