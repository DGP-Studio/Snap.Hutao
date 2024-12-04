// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Abstraction;

internal interface INameQualityAccess : INameAccess
{
    QualityType Quality { get; }
}