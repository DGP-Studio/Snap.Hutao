// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Configuration;

internal enum ChannelOptionsErrorKind
{
    None = 0,
    GamePathNullOrEmpty,
    ConfigurationFileNotFound = 2, // ERROR_FILE_NOT_FOUND
    GameContentCorrupted, // Both configuration file and ScriptVersion file are missing
    SharingViolation = 32, // ERROR_SHARING_VIOLATION
    DeviceNotFound = 433,  // ERROR_NO_SUCH_DEVICE ERROR_NOT_READY
    GamePathLocked,
}