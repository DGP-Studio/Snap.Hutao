﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Core.Caching;

internal interface IImageCacheFilePathOperation
{
    ValueFile GetFileFromCategoryAndName(string category, string fileName);
}