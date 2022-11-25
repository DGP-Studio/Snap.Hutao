// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;

namespace Snap.Hutao.Web.Bridge;

/// <summary>
/// 调用桥暴露的COM接口
/// </summary>
[ComVisible(true)]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IMiHoYoJsBridge
{
    /// <summary>
    /// 消息发生时调用
    /// </summary>
    /// <param name="str">消息</param>
    void OnMessage(string str);
}
