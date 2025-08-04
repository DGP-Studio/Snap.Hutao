// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

internal enum YaeCommandKind : byte
{
    None = 0,
    ResponseAchievement = 1,
    ResponsePlayerStore = 2,
    ResponsePlayerProp = 3,
    RequestCmdId = 252,
    RequestRva = 253,
    RequestResumeThread = 254,
    SessionEnd = 255,
}