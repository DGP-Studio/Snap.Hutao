// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Snap.Hutao.Control;

namespace Snap.Hutao.ViewModel.Wiki;

internal interface ITokenizingTextBoxAccessor : IXamlElementAccessor
{
    TokenizingTextBox TokenizingTextBox { get; }
}
