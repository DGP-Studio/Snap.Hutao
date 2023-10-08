// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder;

[Injection(InjectAs.Singleton)]
internal class JsonHttpContentSerializer : HttpContentSerializer
{
    public JsonHttpContentSerializer(JsonSerializerOptions jsonSerializerOptions)
    {
        JsonSerializerOptions = jsonSerializerOptions;
    }

    internal JsonSerializerOptions? JsonSerializerOptions { get; set; }

    protected override HttpContent? SerializeCore(object? content, Type contentType, Encoding encoding)
    {
        // Note for the future:
        // It can happen that content.GetType() is more specific than contentType, e.g.
        //
        // content.GetType(): string
        // contentType:       object
        //
        // This can happen with the generic Serialize<T> method, if T is less specific than the actual object.
        //
        // .NET's JsonSerializer doesn't serialize all properties of the sub class in this case,
        // because it doesn't have any type info about them (after all, the type of the sub class is unknown).
        // This seems to be intended, because the serializer's generic overload actually documents this.
        // It's a major difference to Newtonsoft.Json though, so people who switch libraries may find this weird,
        // even though this is certainly not a bug of this code here.
        //
        // If this ever turns out to be a problem, it's possible to overwrite contentType with the more specific
        // content.GetType() here.
        // If this is being considered, ensure that this is a feature flag.

        // Since .NET's Json members are optimized for UTF-8, it makes sense to use these
        // optimizations if that's the encoding as well.
        if (encoding == Encoding.UTF8)
        {
            return SerializeUtf8(content, contentType);
        }
        else
        {
            return SerializeOtherEncoding(content, contentType, encoding);
        }
    }

    protected override async ValueTask<object?> DeserializeAsyncCore(HttpContent? httpContent, Type contentType, CancellationToken cancellationToken)
    {
        // While we'd ideally read directly from the stream here (if UTF-8), this is a lot of messy work.
        // For one, we don't know if the encoding is UTF-8 and finding this out is hard to do
        // with tasks like checking the BOM and charset format.
        // As a reference, check out https://github.com/dotnet/corefx/blob/7e9a177824cbefaee8985a9b517ebb0ea2e17a81/src/System.Net.Http/src/System/Net/Http/HttpContent.cs#L604
        // For the moment, simply fall back to simple string deserialization and optimize this later,
        // if required.
        // If this is an absolute must, people can still derive from this class and override the method.
        string json = httpContent is null ? string.Empty : await httpContent.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize(json, contentType, JsonSerializerOptions);
    }

    private HttpContent? SerializeUtf8(object? content, Type contentType)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(content, contentType, JsonSerializerOptions);
        ByteArrayContent httpContent = new(bytes);
        MediaTypeHeaderValue contentTypeHeader = new(MediaType.ApplicationJson)
        {
            CharSet = Encoding.UTF8.WebName,
        };

        httpContent.Headers.ContentType = contentTypeHeader;
        return httpContent;
    }

    private HttpContent? SerializeOtherEncoding(object? content, Type contentType, Encoding encoding)
    {
        string str = JsonSerializer.Serialize(content, contentType, JsonSerializerOptions);
        return new StringContent(str, encoding, MediaType.ApplicationJson);
    }
}