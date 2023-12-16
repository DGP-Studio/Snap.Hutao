using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public sealed class JsonSerializeTest
{
    private readonly JsonSerializerOptions AlowStringNumberOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    private const string SmapleObjectJson = """
        {
            "A" :1
        }
        """;

    private const string SmapleEmptyStringObjectJson = """
        {
            "A" : ""
        }
        """;

    private const string SmapleNumberKeyDictionaryJson = """
        {
            "111" : "12",
            "222" : "34"
        }
        """;

    [TestMethod]
    public void DelegatePropertyCanSerialize()
    {
        SampleDelegatePropertyClass sample = JsonSerializer.Deserialize<SampleDelegatePropertyClass>(SmapleObjectJson)!;
        Assert.AreEqual(sample.B, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void EmptyStringCannotSerializeAsNumber()
    {
        SampleStringReadWriteNumberPropertyClass sample = JsonSerializer.Deserialize<SampleStringReadWriteNumberPropertyClass>(SmapleEmptyStringObjectJson)!;
        Assert.AreEqual(sample.A, 0);
    }

    [TestMethod]
    public void NumberStringKeyCanSerializeAsKey()
    {
        Dictionary<int, string> sample = JsonSerializer.Deserialize<Dictionary<int, string>>(SmapleNumberKeyDictionaryJson, AlowStringNumberOptions)!;
        Assert.AreEqual(sample[111], "12");
    }

    [TestMethod]
    public void ByteArraySerializeAsBase64()
    {
        SampleByteArrayPropertyClass sample = new()
        {
            Array = [1, 2, 3, 4, 5],
        };

        string result = JsonSerializer.Serialize(sample);
        Assert.AreEqual(result, """{"Array":"AQIDBAU="}""");
    }

    private sealed class SampleDelegatePropertyClass
    {
        public int A { get => B; set => B = value; }
        public int B { get; set; }
    }

    private sealed class SampleStringReadWriteNumberPropertyClass
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public int A { get; set; }
    }

    private sealed class SampleByteArrayPropertyClass
    {
        public byte[]? Array { get; set; }
    }
}