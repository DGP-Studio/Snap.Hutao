// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.OrTools.ConstraintSolver;
using Snap.Hutao.Win32.Security;
using Windows.ApplicationModel.VoiceCommands;

namespace Snap.Hutao.Win32.Foundation;

internal struct PSID
{
    public unsafe void* Value;

    public static unsafe implicit operator PSID(SID* value) => *(PSID*)&value;

    public static unsafe implicit operator void*(PSID value) => *(void**)&value;
}