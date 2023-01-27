// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Binding.LaunchGame;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.Unlocker;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏文件操作异常
/// </summary>
internal class GameFileOperationException : Exception
{
    /// <summary>
    /// 构造一个新的用户数据损坏异常
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="innerException">内部错误</param>
    public GameFileOperationException(string message, Exception innerException)
        : base($"游戏文件操作失败: {message}", innerException)
    {
    }
}
