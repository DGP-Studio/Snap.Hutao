// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Encodings.Web;

namespace Snap.Hutao.Core.Json;

/// <summary>
/// 替换 =
/// </summary>
internal class JsonTextEncoder : JavaScriptEncoder
{
    private static readonly string BackSlashDoubleQuote = "\\\"";

    /// <inheritdoc/>
    public override int MaxOutputCharactersPerInputCharacter { get => 6; }

    /// <inheritdoc/>
    public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
    {
        Span<char> textSpan = new(text, textLength);
        return textSpan.IndexOf('=');
    }

    /// <inheritdoc/>
    public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
    {
        // " => \"
        if (unicodeScalar == '"')
        {
            numberOfCharactersWritten = 2;
            return BackSlashDoubleQuote.AsSpan().TryCopyTo(new Span<char>(buffer, bufferLength));
        }

        string encoded = $"\\u{(uint)unicodeScalar:x4}";
        numberOfCharactersWritten = (encoded.Length <= (uint)bufferLength) ? encoded.Length : 0;
        return encoded.AsSpan().TryCopyTo(new Span<char>(buffer, bufferLength));
    }

    /// <inheritdoc/>
    public override bool WillEncode(int unicodeScalar)
    {
        return unicodeScalar == '=';
    }
}
