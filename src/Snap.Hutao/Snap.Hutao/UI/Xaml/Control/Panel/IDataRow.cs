// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

internal interface IDataRow
{
    ImmutableArray<double> ColumnsLength { get; set; }
}