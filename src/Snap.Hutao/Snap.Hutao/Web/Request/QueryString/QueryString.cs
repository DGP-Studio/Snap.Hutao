// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;

namespace Snap.Hutao.Web.Request.QueryString;

/// <summary>
/// querystring serializer/deserializer
/// </summary>
public readonly struct QueryString
{
    private readonly Dictionary<string, List<string>> dictionary = new();

    /// <summary>
    /// Nothing
    /// </summary>
    public QueryString()
    {
    }

    /// <summary>
    /// <para>Gets the first value of the first parameter with the matching name. </para>
    /// <para>Throws <see cref="KeyNotFoundException"/> if a parameter with a matching name could not be found. </para>
    /// <para>O(n) where n = Count of the current object.</para>
    /// </summary>
    /// <param name="name">The parameter name to find.</param>
    /// <returns>query</returns>
    public string this[string name]
    {
        get
        {
            if (TryGetValue(name, out string? value))
            {
                return value;
            }

            throw new KeyNotFoundException($"A parameter with name '{name}' could not be found.");
        }
    }

    /// <summary>
    /// Parses a query string into a <see cref="QueryString"/> object. Keys/values are automatically URL decoded.
    /// </summary>
    /// <param name="queryString">
    /// The query string to deserialize.
    /// Valid input would be something like "a=1&amp;b=5".
    /// URL decoding of keys/values is automatically performed.
    /// Also supports query strings that are serialized using ; instead of &amp;, like "a=1;b=5"</param>
    /// <returns>A new QueryString represents the url query</returns>
    public static QueryString Parse(string? queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
        {
            return new QueryString();
        }

        int questionMarkIndex = queryString.IndexOf('?');
        queryString = queryString[(questionMarkIndex + 1)..];

        string[] pairs = queryString.Split('&', ';');
        QueryString answer = new();
        foreach (string pair in pairs)
        {
            string name;
            string? value;
            int indexOfEquals = pair.IndexOf('=');
            if (indexOfEquals == -1)
            {
                name = pair;
                value = string.Empty;
            }
            else
            {
                name = pair[..indexOfEquals];
                value = pair[(indexOfEquals + 1)..];
            }

            answer.Add(name, value);
        }

        return answer;
    }

    /// <summary>
    /// Gets the first value of the first parameter with the matching name. If no parameter with a matching name exists, returns false.
    /// </summary>
    /// <param name="name">The parameter name to find.</param>
    /// <param name="value">The parameter's value will be written here once found.</param>
    /// <returns>value</returns>
    public bool TryGetValue(string name, [NotNullWhen(true)] out string? value)
    {
        if (dictionary.TryGetValue(name, out List<string>? values))
        {
            value = values.First();
            return true;
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Gets the values of the parameter with the matching name. If no parameter with a matching name exists, sets <paramref name="values"/> to null and returns false.
    /// </summary>
    /// <param name="name">The parameter name to find.</param>
    /// <param name="values">The parameter's values will be written here once found.</param>
    /// <returns>values</returns>
    public bool TryGetValues(string name, [NotNullWhen(true)] out string?[]? values)
    {
        if (dictionary.TryGetValue(name, out List<string>? storedValues))
        {
            values = storedValues.ToArray();
            return true;
        }

        values = null;
        return false;
    }

    /// <summary>
    /// Returns the count of parameters in the current query string.
    /// </summary>
    /// <returns>count of the queries</returns>
    public int Count()
    {
        return dictionary.Select(i => i.Value.Count).Sum();
    }

    /// <summary>
    /// Adds a query string parameter to the query string.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The optional value of the parameter.</param>
    public void Add(string name, string value)
    {
        if (!dictionary.TryGetValue(name, out List<string>? values))
        {
            values = new List<string>();
            dictionary[name] = values;
        }

        values.Add(value);
    }

    /// <inheritdoc cref="Set(string, string)"/>
    public void Set(string name, object value)
    {
        Set(name, value.ToString() ?? string.Empty);
    }

    /// <summary>
    /// Sets a query string parameter. If there are existing parameters with the same name, they are removed.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The optional value of the parameter.</param>
    public void Set(string name, string value)
    {
        dictionary[name] = new() { value };
    }

    /// <summary>
    /// Determines if the query string contains at least one parameter with the specified name.
    /// </summary>
    /// <param name="name">The parameter name to look for.</param>
    /// <returns>True if the query string contains at least one parameter with the specified name, else false.</returns>
    public bool Contains(string name)
    {
        return dictionary.ContainsKey(name);
    }

    /// <summary>
    /// Determines if the query string contains a parameter with the specified name and value.
    /// </summary>
    /// <param name="name">The parameter name to look for.</param>
    /// <param name="value">The value to look for when the name has been matched.</param>
    /// <returns>True if the query string contains a parameter with the specified name and value, else false.</returns>
    public bool Contains(string name, string value)
    {
        return dictionary.TryGetValue(name, out List<string>? values) && values.Contains(value);
    }

    /// <summary>
    /// Removes the first parameter with the specified name.
    /// </summary>
    /// <param name="name">The name of parameter to remove.</param>
    /// <returns>True if the parameters were removed, else false.</returns>
    public bool Remove(string name)
    {
        if (dictionary.TryGetValue(name, out List<string>? values))
        {
            if (values.Count == 1)
            {
                dictionary.Remove(name);
            }
            else
            {
                values.RemoveAt(0);
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes all parameters with the specified name.
    /// </summary>
    /// <param name="name">The name of parameters to remove.</param>
    /// <returns>True if the parameters were removed, else false.</returns>
    public bool RemoveAll(string name)
    {
        return dictionary.Remove(name);
    }

    /// <summary>
    /// Removes the first parameter with the specified name and value.
    /// </summary>
    /// <param name="name">The name of the parameter to remove.</param>
    /// <param name="value">value</param>
    /// <returns>True if parameter was removed, else false.</returns>
    public bool Remove(string name, string value)
    {
        if (dictionary.TryGetValue(name, out List<string>? values))
        {
            if (values.RemoveFirstWhere(i => Equals(i, value)))
            {
                // If removed last value, remove the key
                if (values.Count == 0)
                {
                    dictionary.Remove(name);
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes all parameters with the specified name and value.
    /// </summary>
    /// <param name="name">The name of parameters to remove.</param>
    /// <param name="value">The value to match when deciding whether to remove.</param>
    /// <returns>The count of parameters removed.</returns>
    public int RemoveAll(string name, string value)
    {
        if (dictionary.TryGetValue(name, out List<string>? values))
        {
            int countRemoved = values.RemoveAll(i => Equals(i, value));

            // If removed last value, remove the key
            if (values.Count == 0)
            {
                dictionary.Remove(name);
            }

            return countRemoved;
        }

        return 0;
    }

    /// <summary>
    /// Serializes the key-value pairs into a query string, using the default &amp; separator.
    /// Produces something like "a=1&amp;b=5".
    /// URL encoding of keys/values is automatically performed.
    /// Null values are not written (only their key is written).
    /// </summary>
    /// <returns>query</returns>
    public override string ToString()
    {
        return string.Join('&', GetParameters());
    }

    private IEnumerable<QueryStringParameter> GetParameters()
    {
        foreach ((string name, List<string> values) in dictionary)
        {
            foreach (string value in values)
            {
                yield return new QueryStringParameter(name, value);
            }
        }
    }
}
