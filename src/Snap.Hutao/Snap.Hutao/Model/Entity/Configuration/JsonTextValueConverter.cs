// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Snap.Hutao.Core.Json;

namespace Snap.Hutao.Model.Entity.Configuration;

/// <summary>
/// Json文本转换器
/// </summary>
/// <typeparam name="TProperty">实体类型</typeparam>
[HighQuality]
internal sealed class JsonTextValueConverter<TProperty> : ValueConverter<TProperty, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonTextValueConverter{TProperty}"/> class.
    /// </summary>
    public JsonTextValueConverter()
        : base(
            obj => JsonSerializer.Serialize(obj, JsonOptions.Default),
            str => JsonSerializer.Deserialize<TProperty>(str, JsonOptions.Default)!)
    {
    }
}