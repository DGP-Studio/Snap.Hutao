// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.SuggestBox;

internal interface ITokenizable
{
    string Name { get; }

    ISearchToken Tokenize();
}
