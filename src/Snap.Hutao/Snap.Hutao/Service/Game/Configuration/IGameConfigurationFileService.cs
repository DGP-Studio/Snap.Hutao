// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Configuration;

internal interface IGameConfigurationFileService
{
    void Backup(string source, bool isOversea);

    void Restore(string destination, bool isOversea);
}