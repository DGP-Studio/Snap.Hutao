// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 指示此客户端使用动态密钥
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class UseDynamicSecretAttribute : Attribute
{
}