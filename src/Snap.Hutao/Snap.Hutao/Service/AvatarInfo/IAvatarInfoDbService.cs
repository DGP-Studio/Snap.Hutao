// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using EnkaAvatarInfo = Snap.Hutao.Web.Enka.Model.AvatarInfo;
using ModelAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

internal interface IAvatarInfoDbService
{
    void DeleteAvatarInfoRangeByUid(string uid);

    List<EnkaAvatarInfo> GetAvatarInfoInfoListByUid(string uid);

    List<ModelAvatarInfo> GetAvatarInfoListByUid(string uid);
}