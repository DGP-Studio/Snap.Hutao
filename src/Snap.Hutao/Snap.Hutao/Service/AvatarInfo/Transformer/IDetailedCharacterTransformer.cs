// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace Snap.Hutao.Service.AvatarInfo.Transformer;

internal interface IDetailedCharacterTransformer<in TSource>
{
    void Transform(ref readonly DetailedCharacter detailedCharacter, TSource source);
}