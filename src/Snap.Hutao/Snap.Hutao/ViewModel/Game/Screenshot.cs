// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding;
using Snap.Hutao.Service.Game;
using System.IO;
using System.Text;

namespace Snap.Hutao.ViewModel.Game;

/// <summary>
/// 截图
/// </summary>
internal sealed class Screenshot : INameIcon
{
    /// <summary>
    /// 构造一个新的截图
    /// </summary>
    /// <param name="path">路径</param>
    public Screenshot(string path)
    {
        Name = $"{new FileInfo(path).CreationTime}";
        Icon = path.ToUri();
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public Uri Icon { get; }
}