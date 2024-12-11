// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Calculable;

internal interface ITypedCalculableSource<out TResult, in TType>
    where TResult : ICalculable
{
    public TResult ToCalculable(TType param);
}