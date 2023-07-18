// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 流复制状态
/// </summary>
internal sealed record StreamCopyStatus(long BytesCopied, long TotalBytes);