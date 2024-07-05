// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Calculable;

internal interface ICalculableSource<TResult>
    where TResult : ICalculable
{
    public TResult ToCalculable();
}

internal interface ITypedCalculableSource<TResult, TIndex>
    where TResult : ICalculable
{
    public TResult ToCalculable(TIndex param);
}