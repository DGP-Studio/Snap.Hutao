// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge;

[ExtendedEnum]
internal enum BridgeShareSaveType
{
    [LocalizationKey(nameof(SH.WebBridgeShareSaveKindCopyToClipboard))]
    CopyToClipboard,

    [LocalizationKey(nameof(SH.WebBridgeShareSaveKindSaveAsFile))]
    SaveAsFile,
}