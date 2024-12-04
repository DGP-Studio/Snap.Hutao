// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("object_cache")]
internal sealed class ObjectCacheEntry
{
    [Key]
    public string Key { get; set; } = default!;

    public DateTimeOffset ExpireTime { get; set; }

    [NotMapped]
    public bool IsExpired { get => ExpireTime < DateTimeOffset.Now; }

    public string? Value { get; set; }
}