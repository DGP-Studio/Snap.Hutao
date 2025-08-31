// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

[Flags]
internal enum LocalizationSource
{
    None = 0b0000,
    Hutao = 0b0001,
    Crowdin = 0b0010,
    Gemini = 0b0100,
}