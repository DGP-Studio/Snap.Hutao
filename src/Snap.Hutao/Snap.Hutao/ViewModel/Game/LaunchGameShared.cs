﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.ViewModel.Game;

internal static class LaunchGameShared
{
    public static LaunchScheme? GetCurrentLaunchSchemeFromConfigFile(IGameServiceFacade gameService, IInfoBarService infoBarService)
    {
        ChannelOptions options = gameService.GetChannelOptions();

        if (options.ErrorKind is ChannelOptionsErrorKind.None)
        {
            try
            {
                return KnownLaunchSchemes.Get().Single(scheme => scheme.Equals(options));
            }
            catch (InvalidOperationException)
            {
                if (!IgnoredInvalidChannelOptions.Contains(options))
                {
                    // 后台收集
                    throw ThrowHelper.NotSupported($"不支持的 MultiChannel: {options}");
                }
            }
        }
        else
        {
            infoBarService.Warning($"{options.ErrorKind}", SH.FormatViewModelLaunchGameMultiChannelReadFail(options.FilePath));
        }

        return default;
    }
}