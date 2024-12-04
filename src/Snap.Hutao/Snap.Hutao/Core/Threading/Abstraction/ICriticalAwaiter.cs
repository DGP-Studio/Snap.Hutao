// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Threading.Abstraction;

internal interface ICriticalAwaiter : IAwaiter, ICriticalNotifyCompletion;

internal interface ICriticalAwaiter<out TResult> : IAwaiter<TResult>, ICriticalNotifyCompletion;