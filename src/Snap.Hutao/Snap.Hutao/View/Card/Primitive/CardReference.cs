// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.View.Card.Primitive;

/// <summary>
/// 保存对卡片的引用
/// ItemsRepeater 无法直接使用带有 DataContext 属性的类为直接的源
/// </summary>
internal sealed class CardReference
{
    public Button? Card { get; set; }
}