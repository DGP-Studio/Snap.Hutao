// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Calculable;

internal interface ICalculableSource<out TResult>
    where TResult : ICalculable
{
    public TResult ToCalculable();
}