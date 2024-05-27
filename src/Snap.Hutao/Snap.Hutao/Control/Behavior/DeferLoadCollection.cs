// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Windows.Foundation.Metadata;

namespace Snap.Hutao.Control.Behavior;

[CreateFromString(MethodName = "Snap.Hutao.Control.Behavior.DeferLoadCollection.Parse")]
internal sealed class DeferLoadCollection : List<string>
{
    public static DeferLoadCollection Parse(string text)
    {
        DeferLoadCollection collection = [];
        foreach (StringSegment segment in new StringTokenizer(text, [',']))
        {
            if (segment.HasValue)
            {
                collection.Add(segment.Value);
            }
        }

        return collection;
    }
}