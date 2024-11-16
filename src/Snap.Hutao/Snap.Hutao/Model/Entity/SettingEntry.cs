// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("settings")]
[SuppressMessage("", "SA1124")]
internal sealed partial class SettingEntry
{
    public SettingEntry(string key, string? value)
    {
        Key = key;
        Value = value;
    }

    [Key]
    public string Key { get; set; }

    public string? Value { get; set; }
}