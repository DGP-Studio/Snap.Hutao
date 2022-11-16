// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Snap.Hutao.Core.Json;

/// <summary>
/// 替换 =
/// </summary>
internal class InboxTextEncoder : JavaScriptEncoder
{
    /// <inheritdoc/>
    public override int MaxOutputCharactersPerInputCharacter { get => 6; }

    /// <inheritdoc/>
    public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
    {
        for (int i = 0; i < textLength; i++)
        {
            char c = text[i];
            if (c == '=')
            {
                return i;
            }
        }

        return -1;
    }

    /// <inheritdoc/>
    public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
    {
        string encoded = FormattableString.Invariant($"\\u{(uint)unicodeScalar:x4}");
        numberOfCharactersWritten = (encoded.Length <= (uint)bufferLength) ? encoded.Length : 0;
        return encoded.AsSpan().TryCopyTo(new Span<char>(buffer, bufferLength));
    }

    /// <inheritdoc/>
    public override bool WillEncode(int unicodeScalar)
    {
        return true;
    }
}
