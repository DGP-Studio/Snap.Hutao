// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Control.Builder.ButtonBase;

internal interface IButtonBaseBuilder<TButton> : IBuilder
    where TButton : Microsoft.UI.Xaml.Controls.Primitives.ButtonBase
{
    TButton Button { get; }
}