// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics;

namespace Snap.Hutao.Core.Json;

internal sealed class JsonExtensionDataDictionary : IDictionary<string, JsonElement>
{
    private readonly Dictionary<string, JsonElement> inner = [];
    private readonly FrozenSet<string> ignoredKeys;

    public JsonExtensionDataDictionary(IEnumerable<string> ignoredKeys)
    {
        this.ignoredKeys = ignoredKeys.ToFrozenSet();
    }

    public ICollection<string> Keys
    {
        get => inner.Keys;
    }

    public ICollection<JsonElement> Values
    {
        get => inner.Values;
    }

    public int Count
    {
        get => inner.Count;
    }

    public bool IsReadOnly
    {
        get => false;
    }

    public JsonElement this[string key]
    {
        get => inner[key];
        set => inner[key] = value;
    }

    public void Add(string key, JsonElement value)
    {
        inner.Add(key, value);
        if (!ignoredKeys.Contains(key))
        {
            Debugger.Break();
        }
    }

    public void Add(KeyValuePair<string, JsonElement> item)
    {
        ((ICollection<KeyValuePair<string, JsonElement>>)inner).Add(item);
    }

    public void Clear()
    {
        inner.Clear();
    }

    public bool Contains(KeyValuePair<string, JsonElement> item)
    {
        return ((ICollection<KeyValuePair<string, JsonElement>>)inner).Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return inner.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, JsonElement>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, JsonElement>>)inner).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, JsonElement>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, JsonElement>>)inner).GetEnumerator();
    }

    public bool Remove(string key)
    {
        return inner.Remove(key);
    }

    public bool Remove(KeyValuePair<string, JsonElement> item)
    {
        return ((ICollection<KeyValuePair<string, JsonElement>>)inner).Remove(item);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out JsonElement value)
    {
        return ((IDictionary<string, JsonElement>)inner).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return inner.GetEnumerator();
    }
}